using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Orchard.Admin;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Display;
using Orchard.ContentManagement.Records;
using Orchard.DisplayManagement.Layout;
using Orchard.DisplayManagement.ModelBinding;
using Orchard.Environment.Cache;
using Orchard.Layers.Handlers;
using Orchard.Layers.Models;
using Orchard.Scripting;
using Orchard.Utility;
using YesSql.Core.Services;

namespace Orchard.Layers.Services
{
    public class LayerFilter : IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly ISession _session;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IModelUpdaterAccessor _modelUpdaterAccessor;
        private readonly IScriptingManager _scriptingManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly ISignal _signal;

        public LayerFilter(
            ISession session,
            ILayoutAccessor layoutAccessor,
            IContentItemDisplayManager contentItemDisplayManager,
            IModelUpdaterAccessor modelUpdaterAccessor,
            IScriptingManager scriptingManager,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            ISignal signal)
        {
            _session = session;
            _layoutAccessor = layoutAccessor;
            _contentItemDisplayManager = contentItemDisplayManager;
            _modelUpdaterAccessor = modelUpdaterAccessor;
            _scriptingManager = scriptingManager;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _signal = signal;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // Should only run on the front-end for a full view
            if (context.Result is ViewResult && !AdminAttribute.IsApplied(context.HttpContext))
            {
                var layerParts = await _memoryCache.GetOrCreateAsync("Orchard.Layers:Layers", async entry =>
                {
                    entry.AddExpirationToken(_signal.GetToken(LayerHandler.LayerChangeToken));
                    
                    var layers = await _session.QueryAsync<ContentItem, ContentItemIndex>(x => x.ContentType == "Layer" && x.Published).List();

                    // Apply layers in order
                    return layers
                        .Select(x => x.As<LayerPart>())
                        .Where(x => x != null)
                        .OrderBy(x => x.Order);
                });

                var layout = _layoutAccessor.GetLayout();
                var updater = _modelUpdaterAccessor.ModelUpdater;

                var engine = _scriptingManager.GetScriptingEngine("js");
                var scope = engine.CreateScope(_scriptingManager.GlobalMethodProviders.SelectMany(x => x.GetMethods()), _serviceProvider);

                foreach (var layerPart in layerParts)
                {
                    if (String.IsNullOrEmpty(layerPart.Rule))
                    {
                        continue;
                    }

                    var display = Convert.ToBoolean(engine.Evaluate(scope, layerPart.Rule));

                    if (!display)
                    {
                        continue;
                    }

                    foreach (var zone in layerPart.Widgets.Keys)
                    {
                        foreach (var widget in layerPart.Widgets[zone])
                        {
                            var layerMetadata = widget.As<LayerMetadata>();

                            if (layerMetadata != null)
                            {
                                var widgetContent = await _contentItemDisplayManager.BuildDisplayAsync(widget, updater);

                                widgetContent.Classes.Add("widget");
                                widgetContent.Classes.Add("widget-" + widget.ContentItem.ContentType.HtmlClassify());

                                var contentZone = layout.Zones[zone];
                                contentZone.Add(widgetContent);
                            }
                        }
                    }
                }
            }

            await next.Invoke();
        }
    }
}

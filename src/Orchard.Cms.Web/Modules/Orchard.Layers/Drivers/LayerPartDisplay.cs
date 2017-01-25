using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Display;
using Orchard.ContentManagement.Display.ContentDisplay;
using Orchard.ContentManagement.Display.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.DisplayManagement.Views;
using Orchard.Layers.Models;
using Orchard.Layers.ViewModels;
using Orchard.Settings;

namespace Orchard.Layers.Drivers
{
    public class LayerPartDisplay : ContentPartDisplayDriver<LayerPart>
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISiteService _siteService;

        public LayerPartDisplay(
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            IServiceProvider serviceProvider,
            ISiteService siteService
            )
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _serviceProvider = serviceProvider;
            _siteService = siteService;
        }
        
        public override IDisplayResult Edit(LayerPart layerPart, BuildPartEditorContext context)
        {
            return Shape<LayerPartEditViewModel>("LayerPart_Edit", async m =>
            {
                m.AvailableZones = (await _siteService.GetSiteSettingsAsync()).As<LayerSettings>()?.Zones ?? Array.Empty<string>();
                
                m.LayerPart = layerPart;
                m.Updater = context.Updater;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(LayerPart part, BuildPartEditorContext context)
        {
            var contentItemDisplayManager = _serviceProvider.GetRequiredService<IContentItemDisplayManager>();

            var model = new LayerPartEditViewModel { LayerPart = part };

            await context.Updater.TryUpdateModelAsync(model, Prefix);

            // Explicitly update the Rule property as the LayerPart won't be bound automaticaly
            await context.Updater.TryUpdateModelAsync(model.LayerPart, Prefix, m => m.Rule);

            part.Widgets.Clear();

            // Remove any content or the zones would be merged and not be cleared
            part.Content.Widgets.RemoveAll();

            for (var i = 0; i < model.Prefixes.Length; i++)
            {
                var contentType = model.ContentTypes[i];
                var zone = model.Zones[i];
                var prefix = model.Prefixes[i];

                var contentItem = _contentManager.New(contentType);

                contentItem.Weld(new LayerMetadata());

                var widgetModel = await contentItemDisplayManager.UpdateEditorAsync(contentItem, context.Updater, htmlFieldPrefix: prefix);

                if (!part.Widgets.ContainsKey(zone))
                {
                    part.Widgets.Add(zone, new List<ContentItem>());
                }

                part.Widgets[zone].Add(contentItem);
            }

            return Edit(part, context);
        }
    }
}

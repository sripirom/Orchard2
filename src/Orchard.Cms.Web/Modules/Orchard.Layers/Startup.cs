using System;
using Layers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Display.ContentDisplay;
using Orchard.Data.Migration;
using Orchard.Environment.Navigation;
using Orchard.Layers.Drivers;
using Orchard.Layers.Models;
using Orchard.Layers.Services;
using Orchard.Scripting;
using Orchard.Settings.Services;

namespace Orchard.Layers
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IFilterMetadata, LayerFilter>();

            // Layer Part
            services.AddScoped<IContentPartDisplayDriver, LayerPartDisplay>();
            services.AddSingleton<ContentPart, LayerPart>();

            services.AddScoped<ISiteSettingsDisplayDriver, LayerSiteSettingsDisplayDriver>();
            services.AddSingleton<ContentPart, LayerMetadata>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<INavigationProvider, AdminMenu>();
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var scriptingManager = serviceProvider.GetRequiredService<IScriptingManager>();
            scriptingManager.GlobalMethodProviders.Add(new DefaultLayersMethodProvider());
        }
    }
}

using System;
using Microsoft.Extensions.Localization;
using Orchard.Environment.Navigation;
using Orchard.Layers.Drivers;

namespace Layers
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Design"], design => design
                    .Add(T["Settings"], settings => settings
                        .Add(T["Layers"], T["Layers"], entry => entry
                            .Action("Index", "Admin", new { area = "Orchard.Settings", groupId = LayerSiteSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )));
        }
    }
}

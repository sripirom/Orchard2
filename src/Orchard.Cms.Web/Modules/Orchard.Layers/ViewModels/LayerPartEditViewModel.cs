using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orchard.DisplayManagement.ModelBinding;
using Orchard.Layers.Models;

namespace Orchard.Layers.ViewModels
{
    public class LayerPartEditViewModel
    {

        public string[] AvailableZones { get; set; } = Array.Empty<string>();

        public string[] Zones { get; set; } = Array.Empty<string>();
        public string[] Prefixes { get; set; } = Array.Empty<string>();
        public string[] ContentTypes { get; set; } = Array.Empty<string>();

        public LayerPart LayerPart { get; set; }

        [BindNever]
        public IUpdateModel Updater { get; set; }
    }
}

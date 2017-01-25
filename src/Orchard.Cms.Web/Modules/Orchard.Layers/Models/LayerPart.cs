using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orchard.ContentManagement;

namespace Orchard.Layers.Models
{
    public class LayerPart : ContentPart
    {
        public string Rule { get; set; }
        public int Order { get; set; }

        [BindNever]
        public Dictionary<string, List<ContentItem>> Widgets { get; } = new Dictionary<string, List<ContentItem>>();
    }
}

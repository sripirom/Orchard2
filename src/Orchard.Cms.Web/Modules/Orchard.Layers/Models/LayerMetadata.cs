using Orchard.ContentManagement;

namespace Orchard.Layers.Models
{
    public class LayerMetadata : ContentPart
    {
        public bool RenderTitle { get; set; }
        public string Position { get; set; }
    }
}

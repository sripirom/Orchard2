using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Cache;

namespace Orchard.Layers.Handlers
{
    public class LayerHandler : ContentHandlerBase
    {
        public const string LayerChangeToken = "Orchard.Layers:Layers";

        private readonly ISignal _signal;

        public LayerHandler(ISignal signal)
        {
            _signal = signal;
        }

        public override void Published(PublishContentContext context)
        {
            SignalLayerChanged(context.ContentItem);
        }

        public override void Removed(RemoveContentContext context)
        {
            SignalLayerChanged(context.ContentItem);
        }

        public override void Unpublished(PublishContentContext context)
        {
            SignalLayerChanged(context.ContentItem);
        }
        
        private void SignalLayerChanged(ContentItem contentItem)
        {
            if (contentItem.ContentType == "Layer")
            {
                _signal.SignalToken(LayerChangeToken);
            }
        }
    }
}
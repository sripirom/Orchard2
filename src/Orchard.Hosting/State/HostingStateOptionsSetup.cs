using Microsoft.Extensions.Options;

namespace Orchard.Hosting.State
{
    /// <summary>
    /// Sets up default options for <see cref="HostingStateOptions"/>.
    /// </summary>
    public class HostingStateOptionsSetup : ConfigureOptions<HostingStateOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HostingStateOptions"/>.
        /// </summary>
        public HostingStateOptionsSetup()
            : base(options => { })
        {
        }
    }
}
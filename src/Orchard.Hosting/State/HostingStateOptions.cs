using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace Orchard.Hosting.State
{
    public class HostingStateOptions
    {
        public IList<IFileProvider> FileProviders { get; }
            = new List<IFileProvider>();
    }
}
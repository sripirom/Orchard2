using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Orchard.Hosting.State;

namespace Orchard.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHostingStateStorage(
            this IServiceCollection services,
            IFileProvider fileProvider)
        {
            return services.Configure<HostingStateOptions>(configureOptions: options =>
            {
                options.FileProviders.Add(fileProvider);
            });
        }
    }
}
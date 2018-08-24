using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(EWork.Areas.Identity.IdentityHostingStartup))]
namespace EWork.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
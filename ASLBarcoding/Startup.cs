using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASLBarcoding.Startup))]
namespace ASLBarcoding
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

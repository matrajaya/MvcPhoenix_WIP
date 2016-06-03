using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcPhoenix.Startup))]

namespace MvcPhoenix
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
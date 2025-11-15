using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASPNetWebAppCrudAndGAuth.Startup))]
namespace ASPNetWebAppCrudAndGAuth
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PlanItGirls.Startup))]
namespace PlanItGirls
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

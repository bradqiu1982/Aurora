using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aurora.Startup))]
namespace Aurora
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}

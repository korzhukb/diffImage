using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DiffImage.Startup))]
namespace DiffImage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

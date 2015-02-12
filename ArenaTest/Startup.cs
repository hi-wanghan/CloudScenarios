using Microsoft.Owin;
using Owin;
//hanwan Startup controls the flow
[assembly: OwinStartupAttribute(typeof(ArenaTest.Startup))]
namespace ArenaTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

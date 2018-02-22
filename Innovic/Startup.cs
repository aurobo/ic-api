using Innovic.App;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Data.Entity;
using System.Web.Http;

[assembly: OwinStartup(typeof(Innovic.Startup))]
namespace Innovic
{
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            InitializeDatabase();

            DataProtectionProvider = app.GetDataProtectionProvider();

            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = new TimeSpan(24, 0, 0),
                Provider = new AuthProvider()
            };

            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private void InitializeDatabase()
        {
            InnovicContext context = new InnovicContext();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<InnovicContext, Migrations.Configuration>());
            context.Database.Initialize(true);
        }
    }
}
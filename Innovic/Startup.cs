using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(Innovic.Startup))]
namespace Innovic
{
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            ConfigureWebApi(config);

            app.UseWebApi(config);
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional }
            );
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = new TimeSpan(24, 0, 0),
                //Provider = new AuthProvider()
            };

            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
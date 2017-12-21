using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(BrowserToWebApp.App_Start.StartUp))]
namespace BrowserToWebApp.App_Start
{
    public class StartUp
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string postLogoutRedirectUrl = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUrl"];

        string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            try
            {
                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                app.UseCookieAuthentication(new CookieAuthenticationOptions());

                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        ClientId = clientId,
                        Authority = authority,
                        PostLogoutRedirectUri = postLogoutRedirectUrl,
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthenticationFailed = context =>
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/Error/message=" + context.Exception.Message);
                                return Task.FromResult(0);
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex : " + ex.Message.ToString());
            }
        }
    }
}
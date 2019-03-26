using System.Threading.Tasks;
using GNIBIRPAndVisaAppointment.GmailClient.Application;
using Microsoft.AspNetCore.Mvc;

namespace GNIBIRPAndVisaAppointment.GmailClient.Web.Controllers
{
    public class AuthorizationController : Controller
    {
        OAuthHelper OAuthHelper;

        public AuthorizationController(OAuthHelper oAuthHelper)
        {
            OAuthHelper = oAuthHelper;
        }

        public IActionResult Authorize()
        {
            if (!OAuthHelper.IsAuthorized)
            {
                return Redirect(OAuthHelper.OAuthURL("/"));
            }

            return Redirect("/");
        }
        
        public async Task<IActionResult> Authorized(string state, string code, string scope)
        {
            await OAuthHelper.LogAuthorizationAsync(code, scope);

            return Redirect(OAuthHelper.GetRequestUrlFromState(state));
        }
    }
}
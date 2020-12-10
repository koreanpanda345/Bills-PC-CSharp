using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BillsPC.Web_Dashboard.Data
{
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        public IDataProtectionProvider Provider { get; set; }

        public AccountController(IDataProtectionProvider provider)
        {
            Provider = provider;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties {RedirectUri = returnUrl}, "Discord");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(returnUrl);
        }
    }
}
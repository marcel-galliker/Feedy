using Feedy_WEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;

namespace Feedy_WEB.Pages
{
    public class ConfigModel : PageModel
    {
        [BindProperty]
        public Config config { get; set; }

        public ConfigModel()
        {
            config = new Config();
            config.name = "configuration";
            config.descrpition = "Description";

            FDGlobals.WebSocket.OnReceive = OnReceive;
        }

        public void OnGet()
        {

        }

        private void OnReceive(string msg)
        {
            Console.WriteLine("ConfigModel.Received >>{0}<<", msg);
        }

        public IActionResult OnAdd()
        {
            /*
            User user = EpsonWeb.Models.User.SelectFirstWhere("Email=" + Entity.Value(Email));
            if (user.UserAdmin==false)
                _signInManager.Delete(HttpContext, Email);
            */
            return RedirectToPage("./index");
        }
    }
}

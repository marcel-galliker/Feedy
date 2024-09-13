using Feedy_WEB.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Feedy_WEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Console.WriteLine("IndexModel.OnGet\n");
            MachineInterface.Send("OnGet");
        }
    }
}

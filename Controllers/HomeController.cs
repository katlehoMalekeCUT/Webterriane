using Microsoft.AspNetCore.Mvc;
using WebTerriane.Models;

namespace WebTerriane.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About() => View();
        public IActionResult Faq() => View();

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactFormModel());
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: send email / save to DB
            ViewData["Success"] = true;
            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

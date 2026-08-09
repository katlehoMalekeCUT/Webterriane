using Microsoft.AspNetCore.Mvc;
using WebTerriane.Models;

namespace WebTerriane.Controllers
{
    public class AccommodationController : Controller
    {
        public IActionResult Index()
        {
            var listings = ListingRepository.GetAll();
            return View(listings);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using WebTerriane.Models;

namespace WebTerriane.Controllers
{
    public class ApplicationController : Controller
    {
        [HttpGet]
        public IActionResult Apply(int? listingId)
        {
            var listing = listingId.HasValue
                ? ListingRepository.GetById(listingId.Value)
                : null;

            var model = new ApplicationFormModel
            {
                ListingId = listing?.Id ?? 0,
                ListingTitle = listing?.Title ?? string.Empty
            };

            ViewBag.Listing = listing;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(ApplicationFormModel model)
        {
            if (model.ListingId <= 0)
                ModelState.AddModelError(string.Empty, "No listing was selected. Please apply from a listing page.");

            if (!ModelState.IsValid)
            {
                ViewBag.Listing = ListingRepository.GetById(model.ListingId);
                return View(model);
            }

            // TODO: save the application to the database and/or email it to the team.
            ViewData["Success"] = true;
            return View(new ApplicationFormModel());
        }
    }
}
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ServiceController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
        public ServiceController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
          
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> AddService()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddService(Service model)
        {
            try
            {
               
                    // If the model is valid, add the service to the database and save changes.
                    _context.Services.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index"); // Redirect to the index action after adding the service.
                

                // If the model is not valid, return the view with validation errors.
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately.
                ModelState.AddModelError("", "An error occurred while adding the service. Please try again.");
                return View(model);
            }
        }


        public JsonResult GetService()
        {
            var brands = _context.Services.ToList();

            return Json(new { data = brands });
        }
    }

}

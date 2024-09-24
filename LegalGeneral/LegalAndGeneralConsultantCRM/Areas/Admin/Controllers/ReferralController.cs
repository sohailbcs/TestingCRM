using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Referrals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ReferralController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
       
        public ReferralController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
            
        }


        
        public async Task<IActionResult> Index()
        {
            var followup = await _context.Referrals.ToListAsync();
            return View(followup);
        }


        public async Task<IActionResult> Create()
        { 
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Referral model)
        {

            try
            {
                _context.Referrals.Add(model);
                await _context.SaveChangesAsync();


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }


        public JsonResult GetReferralData()
        {
            var brands = _context.Referrals.ToList();

            return Json(new { data = brands });
        }


        public async Task<IActionResult> Edit(int id)
        {
            // Find the Referral with the given id
            var referral = await _context.Referrals.FindAsync(id);

            if (referral == null)
            {   
                return NotFound();
            }

            return View(referral);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Referral model)
        {
            try
            {
                if (model.ReferralId > 0)
                {
                    // Editing an existing referral
                    _context.Referrals.Update(model);
                }
                else
                {
                    // Creating a new referral
                    _context.Referrals.Add(model);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Referral data successfully edited.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return View("Error");
            }
        }


         
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var referral = await _context.Referrals.FindAsync(id);

                if (referral == null)
                {
                    return NotFound();
                }

                _context.Referrals.Remove(referral);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Referral deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return View("Error");
            }
        }


    }

}

using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class FollowupController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;

        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager; 

        public FollowupController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var followup = await _context.FollowUps.ToListAsync();
            return View(followup);
        }
        public async Task<IActionResult> Create()
        {

            var users = await _userManager.GetUsersInRoleAsync("Employee");
            ViewBag.UserList = new SelectList(users, "Id", "FirstName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FollowUp model)
        {

            try
            { 
                _context.FollowUps.Add(model);
                await _context.SaveChangesAsync();


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                return NotFound();
            }

             return View(followUp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowUpExists(followUp.FollowUpId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            return View(followUp);
        }
        private bool FollowUpExists(int id)
        {
            return _context.FollowUps.Any(e => e.FollowUpId == id);
        }
    }
}

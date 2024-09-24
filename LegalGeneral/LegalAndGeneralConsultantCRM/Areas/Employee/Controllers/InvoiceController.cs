using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.ViewModels.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class InvoiceController : Controller
	{
		private readonly LegalAndGeneralConsultantCRMContext _context;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public InvoiceController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_userManager = userManager;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> MyInvoice()
        {
			// Get the current user's ID from session
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;

			if (string.IsNullOrEmpty(currentUser))
            {
                // Handle the case where the user is not logged in or the session is expired
                return RedirectToAction("Login", "Account"); // Redirect to login page or handle accordingly
            }

            // Fetch leads associated with the current user
            var leads = await _context.Leads
                .Where(l => l.UserId == currentUser)
                .Select(l => new
                {
                    Id = l.LeadId,
                    FullName = l.FirstName + " " + l.LastName
                })
                .ToListAsync();

            // Pass the list to the view
            ViewBag.LeadList = new SelectList(leads, "Id", "FullName");

            return View();
        }




        public async Task<JsonResult> ConsultationInvoice(int leadId)
        {
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;

			if (string.IsNullOrEmpty(currentUser))
            {
                return Json(new { error = "User not logged in" });
            }

            var lead = await _context.Leads
                .Where(l => l.LeadId == leadId && l.UserId == currentUser)
                .Select(l => new
                {
                    FullName = l.FirstName + " " + l.LastName,
                    Email = l.Email,
                    LeadId = l.LeadId,
                    Gender = l.Gender,
                    PhoneNumber = l.PhoneNumber,
                    Cnic = _context.Students
                        .Where(s => s.LeadId == leadId)
                        .Select(s => s.Cnic)
                        .FirstOrDefault(),
                    ConsultationFees = _context.Consultationfees
                        .Where(cf => cf.LeadId == leadId)
                        .Select(cf => new
                        {
                            Amount = cf.Amount,
                            Date = cf.Date.HasValue ? cf.Date.Value.ToShortDateString() : string.Empty
                        })
                        .ToList(),
                    Status = l.IsEnrolled.HasValue && l.IsEnrolled.Value ? "Enrolled" : "Not Enrolled"
                })
                .FirstOrDefaultAsync();

            if (lead == null)
            {
                return Json(new { error = "Lead not found" });
            }

            // Prepare the data to return
            var result = new
            {
                data = new[]
                {
            new
            {
                FullName = lead.FullName,
                LeadId = lead.LeadId,
                Email = lead.Email,
                Gender = lead.Gender,
                PhoneNumber = lead.PhoneNumber,
                Cnic = lead.Cnic,
                ConsultationFees = lead.ConsultationFees.Select(cf => cf.Amount),
                ConsultationDates = lead.ConsultationFees.Select(cf => cf.Date)
            }
        }
            };

            return Json(result);
        }


        public async Task<IActionResult> PrintInvoice(int leadId)
        {
            // Fetch the lead and related student data asynchronously
            var lead = await _context.Leads
                .Include(l => l.Students) // Include student data
                .ThenInclude(s => s.Deposits) // Include related deposits
                .FirstOrDefaultAsync(l => l.LeadId == leadId);

            if (lead == null)
            {
                return NotFound();
            }

            // Fetch the most recent consultation fee asynchronously
            var consultationFee = await _context.Consultationfees
                .Where(cf => cf.LeadId == leadId)
                .OrderByDescending(cf => cf.Date) // Assuming you want the most recent one
                .FirstOrDefaultAsync();

            // Create and populate the view model
            var model = new PrintInvoiceViewModel
            {
                Lead = lead,
                Student = lead.Students.FirstOrDefault(), // Assuming there is at least one student
                ConsultationFees = consultationFee
            };

            return View("PrintInvoice", model);
        }

    }
}

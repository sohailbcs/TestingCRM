using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class CustomerController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
        public CustomerController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
          
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }


        public JsonResult GetFollowUpData()
        {
            var followUpData = _context.FollowUps
                .Include(f => f.Employee).Include(f => f.Lead).Include(f => f.Lead.Referral)
                .Where(f => f.Status == "Converted Lead")  
                .ToList();

            
                var data = followUpData.Select(f => new
                {
                    LeadId = f.Lead?.LeadId,
                    FirstName = f.Employee.FirstName + " "+ f.Employee.LastName,
                    LastName = f.Employee.LastName,
                    LeadFirstNAme = f.Lead?.FirstName,
                    LeadLastName = f.Lead?.LastName,
                    Email = f.Lead?.Email,
                    PhoneNo = f.Lead?.PhoneNumber,
                    PhoneNumber = f.Employee.PhoneNumber,
                    ReferralName = f.Lead?.Referral?.Name ?? string.Empty,
                    FollowUpStatus = f.Status
                });

                return Json(new { data });
            
           
        }


    }

}

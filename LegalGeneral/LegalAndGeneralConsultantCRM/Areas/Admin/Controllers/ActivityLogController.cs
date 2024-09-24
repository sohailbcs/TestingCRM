using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ActivityLogController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

        public ActivityLogController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Activity()
        {
            var users = await _userManager.GetUsersInRoleAsync("Employee");

 
            ViewBag.UserList = new SelectList(users.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}"
            }), "Id", "FullName");


            return View();
        }

        public async Task<JsonResult> GetUserActivity(string userid)
        {
            try
            {
                // Query to fetch user activity details
                var userActivity = await _context.ActivityLogs
                    .Where(a => a.UserId == userid)
                    .Select(a => new
                    {
                        ActivityLogId = a.ActivityLogId,
                        ActivityLogDate = a.ActivityLogDate,
                        Status = a.Status,
                        Action = a.Action,
                       
                        LeadName = a.Lead.FirstName + " "+ a.Lead.LastName,   
                        LeadPhoneNumber = a.Lead.PhoneNumber,
                        LeadEmail = a.Lead.Email,

                        UserName = a.User.FirstName +" "+ a.User.LastName, 
                        
                    })
                    .ToListAsync();

                return Json(userActivity);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { error = $"Error retrieving user activity: {ex.Message}" });
            }
        }
    }
}

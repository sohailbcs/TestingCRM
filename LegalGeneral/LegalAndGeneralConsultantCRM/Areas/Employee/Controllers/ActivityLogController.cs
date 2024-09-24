using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class ActivityLogController : Controller
	{
		private readonly LegalAndGeneralConsultantCRMContext _context;
 		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public ActivityLogController(LegalAndGeneralConsultantCRMContext context, IWebHostEnvironment webHostEnvironment, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<JsonResult> MyActivityLog()
		{
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;

		 

			if (string.IsNullOrEmpty(currentUser))
			{
				return Json(new { data = new List<ActivityLog>() });
			}

			var activityLogs = await _context.ActivityLogs
											 .Where(al => al.UserId == currentUser)
											 .Include(al => al.Lead)
											 .Include(al => al.User)
											 .Select(al => new
											 {
												 al.ActivityLogId,
												 Status = al.Status,
												 Action =  al.Action,
												 ActivityLogDate =  al.ActivityLogDate,
												 LeadName = al.Lead.FirstName + " " + al.Lead.LastName,
												 LeadPhone = al.Lead.PhoneNumber,
												 
											 })
											 .ToListAsync();

			return Json(new { data = activityLogs });
		}
	}
}

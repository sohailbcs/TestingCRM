using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.ViewModels.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
     
    public class EmployeeDashboardController : Controller
	{
		private readonly LegalAndGeneralConsultantCRMContext _dbContext;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public EmployeeDashboardController(LegalAndGeneralConsultantCRMContext dbContext, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		} // Mark a single notification as read
		[HttpGet]
		public async Task<IActionResult> MarkAsRead(int id)
		{
			var notification = await _dbContext.Notifications.FindAsync(id);
			if (notification == null)
			{
				return NotFound();
			}

			notification.IsRead = true;
			await _dbContext.SaveChangesAsync();
			return RedirectToAction("Index");
		
		}

		// Mark all notifications as read
		// Mark all notifications as read
		[HttpGet]
		public async Task<IActionResult> MarkAllAsRead()
		{
			var user = await _userManager.GetUserAsync(User);
			var currentUserId = user.Id;
			var unreadNotifications = _dbContext.Notifications
				.Where(n => n.UserId == currentUserId && !n.IsRead)
				.ToList();

			foreach (var notification in unreadNotifications)
			{
				notification.IsRead = true;
			}

			await _dbContext.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Index()
		{
			 

			var user = await _userManager.GetUserAsync(User);
			var currentUserId = user.Id;

			if (string.IsNullOrEmpty(currentUserId))
			{
				// Handle the case where user ID is not found in session (not authenticated)
				return RedirectToAction("Login", "Account", new { area = "Identity" });
			}
            // Fetch notifications for the current user where status is false
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == currentUserId && n.IsRead == false)
                .OrderByDescending(n => n.NotificationTime == DateTime.Today)
                .ToListAsync();

            ViewBag.Notifications = notifications; // Pass notifications to the view via ViewBag

            var mylead = await _dbContext.Leads
							.Where(sp => sp.UserId == currentUserId)
							.CountAsync();

			ViewBag.MyLead = mylead;
			var enrolled = await _dbContext.VisaApplications
										.Where(va => va.UserId == currentUserId && va.VisaStatus == "accept")
										.Select(va => va.LeadId)
										.Distinct()
										.CountAsync();
			var leadcount = await _dbContext.Leads.CountAsync();
			var dead = await _dbContext.FollowUps.CountAsync(f => f.Status == "Dead Lead" && f.EmployeeId == currentUserId);
			
			ViewBag.LeadCount = mylead;
			ViewBag.DeadLead = dead;

			var hot = await _dbContext.FollowUps.CountAsync(f => f.Status == "Hot Lead" && f.EmployeeId == currentUserId);
			ViewBag.Hot = hot;

			var metting = await _dbContext.CalendarEvents.CountAsync();
			ViewBag.MEt = metting;

			var cold = await _dbContext.FollowUps.CountAsync(f => f.Status == "Cold Lead" && f.EmployeeId == currentUserId);
			ViewBag.Cold = cold;

			var fut = await _dbContext.FollowUps.CountAsync(f => f.Status == "Future Lead" && f.EmployeeId == currentUserId);
			ViewBag.Fut = fut;

			var currentMonth = DateTime.Now.Month;
			var depo = await _dbContext.Deposits.CountAsync();
			ViewBag.Depo = depo;
			ViewBag.Visa = enrolled;
			var userLeadIds = await _dbContext.Leads
										   .Where(l => l.UserId == currentUserId)
										   .Select(l => l.LeadId)
										   .ToListAsync();
			var hotLeadCount = await _dbContext.FollowUps
											.Where(f => userLeadIds.Contains(f.LeadId ?? 0) && f.Status == "Hot Lead")
											.CountAsync();
			ViewBag.HotLeadCount = hotLeadCount;


			var visa = await _dbContext.VisaApplications
									   .Where(va => va.UserId == currentUserId)
									   .Select(va => va.LeadId)
									   .Distinct()
									   .CountAsync();
			ViewBag.Total = visa;



			var userLeads = await _dbContext.Leads
								   .Where(lead => lead.UserId == currentUserId)
								   .Select(lead => lead.LeadId)
								   .ToListAsync();

			var studentCount = await _dbContext.Students
											  .Where(student => userLeads.Contains(student.LeadId ?? 0))
											  .CountAsync();

			ViewBag.StudentCount = studentCount;





			if (currentUserId == null)
			{
				return Unauthorized();
			}
			var today = DateTime.UtcNow.Date; // Get today's date in UTC
			var activityLogs = await _dbContext.ActivityLogs
							.Where(al => al.UserId == currentUserId && al.ActivityLogDate.HasValue && al.ActivityLogDate.Value.Date == today)
							.OrderBy(al => al.ActivityLogDate) // Order by date to get the latest ones
							.Take(4) // Take only the first 4 records
							.ToListAsync();
			// Find the leads assigned to the current user
			var leads = await _dbContext.Leads
				.Include(l => l.StudentMessages)
				.Where(l => l.UserId == currentUserId)
				.ToListAsync();

			// Create a view model to store the messages
			var messagesVM = new List<MessageVM>();

			foreach (var lead in leads)
			{
				foreach (var studentMessage in lead.StudentMessages)
				{
					var leadActivityLogs = activityLogs
			.Where(al => al.LeadId == lead.LeadId)
			.ToList();
					messagesVM.Add(new MessageVM
					{
						Leads = lead,
						StudentMessages = studentMessage,
						ActivityLogs = leadActivityLogs // Now it's correctly a List<ActivityLog>

					});
				}
			}

			return View(messagesVM);
		}


        public async Task<JsonResult> GetUserActivity()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user.Id;

            // Get today's date
            var today = DateTime.Today;

            // Fetch all follow-ups with a status of "Dead Lead" for today for the current user
            var deadLeads = await _dbContext.ActivityLogs
                .Include(f => f.Lead)
                .Include(f => f.User) // Include the User data
                .Where(f => f.ActivityLogDate.HasValue
                            && f.ActivityLogDate.Value.Date == today
                            && f.UserId == currentUserId) // Filter by current user ID
                .OrderByDescending(f => f.ActivityLogDate) // Optional: order by date
                .Select(f => new
                {
                    f.Lead.LeadId,
                    f.Lead.FirstName,
                    f.Lead.LastName,
                    FullName = f.Lead.FirstName + " " + f.Lead.LastName,
                    LeadPhoneNumber = f.Lead.PhoneNumber,
                    Email = f.Lead.Email,
                    FollowUpStatus = f.Status,
                    UserName = f.User.FirstName + " " + f.User.LastName, // Include the UserName
                    UserId = f.UserId,
                    ActivityLogId = f.ActivityLogId,
                    Action = f.Action,
                    ActivityLogDate = f.ActivityLogDate
                })
                .ToListAsync();

            return Json(new { data = deadLeads });
        }



    }
}

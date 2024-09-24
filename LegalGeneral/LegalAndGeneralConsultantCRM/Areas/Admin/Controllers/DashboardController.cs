using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.ViewModels.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {
		private readonly LegalAndGeneralConsultantCRMContext _dbContext;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public DashboardController(LegalAndGeneralConsultantCRMContext dbContext, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
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
			int convertedLeadCount = GetConvertedLeadCount();

			var leadcount = await _dbContext.Leads.CountAsync();
			var dead = await _dbContext.FollowUps.CountAsync(f => f.Status == "Dead Lead");
			ViewBag.ConvertedLeadCount = convertedLeadCount;
			ViewBag.LeadCount = leadcount;
			ViewBag.DeadLead = dead;

			var hot = await _dbContext.FollowUps.CountAsync(f => f.Status == "Hot Lead");
			ViewBag.Hot = hot;

			var metting = await _dbContext.CalendarEvents.CountAsync();
			ViewBag.MEt = metting;

			var cold = await _dbContext.FollowUps.CountAsync(f => f.Status == "Cold Lead");
			ViewBag.Cold = cold;

			var fut = await _dbContext.FollowUps.CountAsync(f => f.Status == "Future Lead");
			ViewBag.Fut = fut;

			var currentMonth = DateTime.Now.Month;
			var depo = await _dbContext.Deposits.CountAsync();
			ViewBag.Depo = depo;

			var leadCountThisMonth = await _dbContext.Leads
				.Where(lead => lead.CreatedDate.HasValue && lead.CreatedDate.Value.Month == currentMonth)
				.CountAsync();
			ViewBag.LeadCountThisMonth = leadCountThisMonth;

			var studentcount = await _dbContext.Students.CountAsync();
			ViewBag.Studentcount = studentcount;

			var approvedCount = await _dbContext.StudentMessages.CountAsync(f => f.Status == "Approved");
			ViewBag.Enrolled = approvedCount;

			var visacount = await _dbContext.VisaApplications.CountAsync(f => f.VisaStatus == "awaiting");
			ViewBag.Visa = visacount;

			var visareject = await _dbContext.VisaApplications.CountAsync(f => f.VisaStatus == "reject");
			ViewBag.visareject = visareject;

			var depostcount = await _dbContext.Deposits.CountAsync(l => l.Amount != null);
			ViewBag.depostcount = depostcount;

			var vi = await _dbContext.VisaApplications.CountAsync();
			ViewBag.Vis = vi;

			var enrolled = await _dbContext.VisaApplications
										.Where(va => va.VisaStatus == "accept")
										.CountAsync();
			ViewBag.Enrolled = enrolled;

			var branch = await _dbContext.Branches.CountAsync();
			ViewBag.Branch = branch;

			var uni = await _dbContext.Universities.CountAsync();
			ViewBag.Uni = uni;

			var employeeCount = (await _userManager.GetUsersInRoleAsync("Employee")).Count;
			ViewBag.EmployeeCount = employeeCount;

			var totalFees = await _dbContext.Consultationfees
				.Where(f => f.Amount != null)
				.SumAsync(f => f.Amount);
			ViewBag.Fee = totalFees;
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
			return View();
		}

		private int GetConvertedLeadCount()
        {
            // Assuming "Converted Lead" is the status you are looking for
            const string convertedLeadStatus = "Converted Lead";

            // Query the database to get the count of converted leads
            int convertedLeadCount = _dbContext.FollowUps
                .Count(f => f.Status == convertedLeadStatus);

            return convertedLeadCount;
        }
		public async Task<JsonResult> GetUserActivity()
		{
			// Get today's date
			var today = DateTime.Today;

			// Fetch all follow-ups with a status of "Dead Lead" for today
			var deadLeads = await _dbContext.ActivityLogs
				.Include(f => f.Lead)
				.Include(f => f.User) // Include the User data
				.Where(f => f.ActivityLogDate.HasValue && f.ActivityLogDate.Value.Date == today)
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



		public async Task<JsonResult> GetCustomer()
        {
            try
            {
                // Get the current date and the first day of the current month
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                // Query to fetch leads with accepted visa applications in the current month
                var acceptedVisaApplications = await _dbContext.VisaApplications
                    .Where(va => va.VisaStatus == "accept")
                    .Select(va => new
                    {
                        va.Lead.LeadId,
                        LeadName = va.Lead.FirstName + " " + va.Lead.LastName,
                        PhoneNumber =   va.Lead.PhoneNumber,
                        va.VisaStatus,
                        va.SubmissionDate
                    })
                    .ToListAsync();

                return Json(acceptedVisaApplications);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { error = $"Error retrieving customer data: {ex.Message}" });
            }
        }


    }
}

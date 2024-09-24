using LegalAndGeneralConsultantCRM.Areas.Admin.SendNotificationHub;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{

	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class LeadfollowupController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public LeadfollowupController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Employee"))
            {
                // Check if the user has an existing FollowUp with "Converted Lead" or "Dead Lead" status
                var existingFollowUp = _context.FollowUps
                    .FirstOrDefault(f => f.EmployeeId == currentUser.Id && (f.Status == "Converted Lead" || f.Status == "Dead Lead"));

                if (existingFollowUp == null)
                {
                    ViewData["FirstName"] = currentUser.FirstName;
                    return View();
                }
                else
                {
                    // User has an existing FollowUp, disable creation/editing
                    ViewData["CustomErrorKey"] = $"You already have a FollowUp with '{existingFollowUp.Status}' status.";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create(FollowUp model)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return View("Error");
                }

                // Check if a follow-up already exists for the current user
                var existingFollowUp = _context.FollowUps.SingleOrDefault(f => f.EmployeeId == currentUser.Id);

                // Check if an existing follow-up with "Converted Lead" or "Dead Lead" status exists
                var existingConvertedOrDeadLead = _context.FollowUps
                    .Any(f => f.EmployeeId == currentUser.Id && (f.Status == "Converted Lead" || f.Status == "Dead Lead"));

                if (existingFollowUp != null)
                {
                    // Update existing follow-up
                    existingFollowUp.FollowUpDate = model.FollowUpDate; // Update properties accordingly
                    existingFollowUp.Status = model.Status;
                    existingFollowUp.Reminder = model.Reminder;
                    existingFollowUp.Description = model.Description;
                    existingFollowUp.FollowUpCompleted = model.FollowUpCompleted;

                    _context.FollowUps.Update(existingFollowUp);
                }

                else if (!existingConvertedOrDeadLead)
                {
                    // Set the EmployeeId for the new follow-up
                    model.EmployeeId = currentUser.Id;

                    // Add new follow-up
                    _context.FollowUps.Add(model);
                }
                else
                {
                    // Existing "Converted Lead" or "Dead Lead" status found, display disabled form or handle accordingly
                    return View("Error"); // You might want to create a specific view for this case
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log them
                return View("Error");
            }
        }




    }
}

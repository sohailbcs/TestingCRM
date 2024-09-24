using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using LegalAndGeneralConsultantCRM.Areas.Admin.SendNotificationHub;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.ViewModels.Employee;

namespace LegalAndGeneralConsultantCRM.Controllers
{
    public class RegisterController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RegisterController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, RoleManager<IdentityRole> roleManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
        }

        public IActionResult Register()
        {
            var services = _context.Services.ToList();

            // Populate ViewBag with services
            ViewBag.Services = services;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRegister(RegisterViewModel model)
        {
            try
            {
                // Check if the "Customer" role exists, if not, create it
                if (!await _roleManager.RoleExistsAsync("Customer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }

                var user = new LegalAndGeneralConsultantCRMUser
                {
                    FirstName = model.FirstName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    SelectedServiceId = model.SelectedServiceId,
                    UserName = model.Email,
                    CreatedAt = DateTime.UtcNow
                // Add other properties as needed
            };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // If registration is successful, assign the user to the "Customer" role
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // Create and save the notification
                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Message = $"{user.FirstName} has been successfully registered.",
                        IsRead = false // Assuming notifications are initially unread
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    // Send notification using SignalR to Admin group
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", user.FirstName);

                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, and optionally display a user-friendly message
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                // Log the exception ex
            }

            return View("Register", model);

        }
    }
}
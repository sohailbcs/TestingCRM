using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.SendNotificationHub
{
    public class NotificationHub : Hub
    {
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly LegalAndGeneralConsultantCRMContext _context;

        public NotificationHub(UserManager<LegalAndGeneralConsultantCRMUser> userManager, LegalAndGeneralConsultantCRMContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task LoadNotifications()
        {
            try
            {
                var userId = Context.UserIdentifier;
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Retrieve notifications for the user
                    var notifications = await _context.Notifications
                        .Where(n => n.UserId == userId)
                        .OrderByDescending(n => n.NotificationId)
                        .ToListAsync();

                    // Send notifications to the client
                    await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in LoadNotifications: {ex.Message}");
            }
        }
        public async Task MarkNotificationsAsRead()
        {
            try
            {
                var userId = Context.UserIdentifier;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Retrieve notifications for the user
                    var notifications = await _context.Notifications
                         
                        .ToListAsync();

                    // Send notifications to the client
                    await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in MarkNotificationsAsRead: {ex.Message}");
            }
        }
        public async Task NewRegistration()
        {
            var userId = Context.UserIdentifier;
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Retrieve notifications for the user
                var notifications = await _context.Notifications
                   
                    .ToListAsync();

                // Send notifications to the client
                await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
            }

        }


    }
}


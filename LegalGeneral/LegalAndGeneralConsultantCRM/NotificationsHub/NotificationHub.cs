using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LegalAndGeneralConsultantCRM.NotificationsHub
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
				// Get the current user
				var user = await _userManager.GetUserAsync(Context.User);
				var currentUser = user.Id;

				// Retrieve notifications for the user
				var notifications = await _context.Notifications
					.Where(n => n.UserId == currentUser)
					.OrderByDescending(n => n.NotificationId)
					.ToListAsync();

				// Send notifications to the client
				await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
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
				// Get the current user
				var user = await _userManager.GetUserAsync(Context.User);
				var currentUser = user.Id;

				if (!string.IsNullOrEmpty(currentUser))
				{
					// Mark unread notifications as read
					var unreadNotifications = await _context.Notifications
						.Where(n => n.UserId == currentUser && !n.IsRead)
						.ToListAsync();

					foreach (var notification in unreadNotifications)
					{
						notification.IsRead = true;
					}

					await _context.SaveChangesAsync();

					// Inform the client that notifications are marked as read
					await Clients.Caller.SendAsync("NotificationsMarkedAsRead");
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in MarkNotificationsAsRead: {ex.Message}");
			}
		}
	}
}

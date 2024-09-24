using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.CalendarEvents;
using LegalAndGeneralConsultantCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class MeetingController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public MeetingController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult Target()
		{
			var user = _userManager.GetUserAsync(User);
			var currentUserId = user.Result.Id; // Use `Result` to access the user object asynchronously

			var tasks = _context.TargetTask
				.Where(t => t.UserId == currentUserId)
				.ToList();

			var calls = tasks.Where(t => t.TaskType == "Calls").ToList();
			var connectedCalls = tasks.Where(t => t.TaskType == "ConnectedCalls").ToList();
			var visits = tasks.Where(t => t.TaskType == "Visits").ToList();
			var meetings = tasks.Where(t => t.TaskType == "Meetings").ToList();
			var salesAmount = tasks.Where(t => t.TaskType == "SalesAmount").ToList();
			var sales = tasks.Where(t => t.TaskType == "Sales").ToList();

			var followUps = _context.FollowUps
				.Where(f => f.EmployeeId == currentUserId)
				.ToList();

			var callsCount = followUps.Count(f => f.FollowUpType == "Calls");
			var connectedCallsCount = followUps.Count(f => f.FollowUpType == "ConnectedCalls");
			var visitsCount = followUps.Count(f => f.FollowUpType == "Visits");
			var meetingsCount = followUps.Count(f => f.FollowUpType == "Meetings");

			var viewModel = new TargetViewModel
			{
				Calls = calls,
				Visits = visits,
				Meetings = meetings,
				SalesAmount = salesAmount,
				ConnectedCalls = connectedCalls, // Ensure ConnectedCalls is set
				Sales = sales,
				FollowUpCounts = new FollowUpCounts
				{
					CallsCount = callsCount,
					VisitsCount = visitsCount,
					MeetingsCount = meetingsCount,
					ConnectedCallsCount = connectedCallsCount
				}
			};

			return View(viewModel);
		}



		[HttpPost]
		public async Task<IActionResult> GetUnreadTodayEvents()
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
			{
				return Json(new { success = false, message = "User not found" });
			}

			// Get today's date
			var today = DateTime.Today;

			// Retrieve unread events for the current user for today
			var events = await _context.CalendarEvents
				.Where(e => e.UserId == currentUser.Id && e.IsRead == false && e.EventDate.HasValue && e.EventDate.Value.Date == today)
				.Select(e => new
				{
					e.CalendarEventId,
					e.Name,
					e.Description,
					EventDate = e.EventDate.Value.ToString("HH:mm"), // Format time only
					e.ThemeColor
				})
				.ToListAsync();

			if (events.Any())
			{
				return Json(new { success = true, events });
			}

			return Json(new { success = false, message = "No unread events for today" });
		}
		[HttpPost]
		public async Task<IActionResult> MarkAsRead(int id)
		{
			var eventToMark = await _context.CalendarEvents.FindAsync(id);
			if (eventToMark == null)
			{
				return Json(new { success = false, message = "Event not found" });
			}

			eventToMark.IsRead = true;
			_context.CalendarEvents.Update(eventToMark);
			await _context.SaveChangesAsync();

			return Json(new { success = true });
		}

		public IActionResult Index()
        {
            return View();
        }
        public IActionResult Event()
        {
            ViewBag.Leads = _context.Leads.ToList(); // Fetch all leads from the database
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> AddEvent([FromForm] CalendarEventViewModel model)
		{
			var user = await _userManager.GetUserAsync(User);  
			var currentUser = user.Id;  
			CalendarEvent calendarEvent = new CalendarEvent
			{
				EventDate = model.EventDate,
				Description = model.Description,
				ThemeColor = model.ThemeColor,
				Name = model.Name,
				UserId = currentUser,
			};
			var notifications = new Notification
			{
				UserId = currentUser, // EmployeeId is saved as UserId
				Message =model.Description,
				NotificationTime = model.EventDate,
				IsRead = false
			};

			_context.Notifications.Add(notifications);
			_context.CalendarEvents.Add(calendarEvent);
			await _context.SaveChangesAsync();  

			return Json(new { success = true });
		}


		public async Task<IActionResult> GetEvents()
		{
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;  
			var events = await _context.CalendarEvents
				.Where(e => e.UserId == currentUser)  
				.Select(e => new
				{
					id = e.CalendarEventId,
					title = e.Name,
					start = e.EventDate,
					end = e.EventDate,
					description = e.Description,
					color = e.ThemeColor
				})
				.ToListAsync();

			return Json(events);
		}


		[HttpPost]
        public IActionResult UpdateEvent([FromForm] CalendarEventViewModel model)
        {
            
                var eventToUpdate = _context.CalendarEvents.Find(model.CalendarEventId);
            if (eventToUpdate != null)
            {
                eventToUpdate.Name = model.Name;
                eventToUpdate.Description = model.Description;
                eventToUpdate.EventDate = model.EventDate;
                eventToUpdate.ThemeColor = model.ThemeColor;

                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Event not found." });
            }
        }

        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            var eventToDelete = _context.CalendarEvents.Find(id);
            if (eventToDelete != null)
            {
                _context.CalendarEvents.Remove(eventToDelete);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Event not found." });
            }
        }


    }

}

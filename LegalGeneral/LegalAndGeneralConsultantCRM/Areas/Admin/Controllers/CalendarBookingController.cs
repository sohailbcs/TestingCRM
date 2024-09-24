using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.CalendarEvents;
using LegalAndGeneralConsultantCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class CalendarBookingController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;

        public CalendarBookingController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
        }

        // GET: Admin/CalendarBooking/Index
        public IActionResult Index()
        {
            ViewBag.Leads = _context.Leads.ToList(); // Fetch all leads from the database
            return View();
        }

       
        [HttpPost]
        public IActionResult AddEvent([FromForm] CalendarEventViewModel model)
        {

            CalendarEvent calendarEvent = new CalendarEvent
            {

                EventDate = model.EventDate,
                Description = model.Description,
                ThemeColor = model.ThemeColor,
                Name = model.Name,
             };

            _context.CalendarEvents.Add(calendarEvent);
                _context.SaveChanges();

                return Json(new { success = true });
            
            
        }

        // GET: Admin/CalendarBooking/GetEvents
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.CalendarEvents
               
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
    }
}

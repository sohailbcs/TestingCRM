using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.Models.CalendarEvents;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{

	[Area("Employee")]
    [Authorize(Roles = "Employee")]    
    public class MyLeadController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
 
		public MyLeadController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> LeadDetail(int leadId)
        {
            if (leadId <= 0)
            {
                return NotFound();
            }

            // Retrieve all lead history records for the given lead ID
            var leadHistories = await _context.LeadHistories
                .Include(lh => lh.User) // Include related user details if needed
                .Where(lh => lh.LeadId == leadId)
                .ToListAsync();

            if (leadHistories == null || !leadHistories.Any())
            {
                // Pass an empty list to the view if no records found
                return View(new List<LeadHistoryViewModel>());
            }

            // Convert LeadHistory entities to LeadHistoryViewModel
            var leadHistoryViewModels = leadHistories.Select(lh => new LeadHistoryViewModel
            {
                FullName = lh.User?.FirstName, // Assuming User contains FirstName
                PhoneNumber = lh.User?.PhoneNumber, // Assuming User contains PhoneNumber
                Email = lh.User?.Email, // Assuming User contains Email
                Status = lh.Status,
                LastFollowUps = lh.LeadFollowupDate, // Assuming this is the date of the follow-up
                Description = lh.Description // Assuming this is the date of the follow-up
            }).ToList();

            // Pass the converted view models to the view
            return View(leadHistoryViewModels);
        }

        public async Task<IActionResult> Index()
        {
             
            return View();
        }

        public async Task<IActionResult> InProcessLead()
        {

            return View();
        }
        public async Task<JsonResult> GetAllocatedLeadData()
        {
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;

			if (currentUser == null)
            {
                return Json(new { data = new List<object>() });
            }

            var excludedStatuses = new List<string> { "Dead Lead", "Converted Lead" };

            var allocatedLeads = _context.Leads
                      .Where(lead => lead.IsLeadAssign == true &&
                                     lead.Assignees.Any(assignee => assignee.EmployeeId == currentUser) &&
                                     (lead.FollowUps.Any(followUp => followUp.Status == null) ||
                                     !lead.FollowUps.Any(followUp => excludedStatuses.Contains(followUp.Status))))
                      .Include(l => l.Referral)
                      .Include(l => l.Assignees).ThenInclude(lae => lae.Employee)
                      .Include(l => l.FollowUps)
                      .ToList();


            var leadData = allocatedLeads.Select(l => new
            {
                l.LeadId,
                l.FirstName,
                l.LastName,
                FullName = $"{l.FirstName} {l.LastName}",
                l.PhoneNumber,
                l.Email,
                l.Gender,
                l.LeadSource,
                CreatedDate = l.CreatedDate?.Date,
                ReferralName = l.Referral != null ? l.Referral.Name : null,
                EmployeeFirstName = l.Assignees.FirstOrDefault()?.Employee?.FirstName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status,
                FollowUpDate = l.FollowUps.FirstOrDefault()?.FollowUpDate
            });

            return Json(new { data = leadData });
        }

        public async Task<IActionResult> Edit(int id)
        {
            //var brand = await _context.Brands.ToListAsync();
            //var productTypes = await _context.ProductTypes.ToListAsync();
            //var product = await _context.Products.ToListAsync();
            //var interestTags = await _context.InterestedTags.ToListAsync();
            var referrals = await _context.Referrals.ToListAsync();

            //ViewBag.BrandList = new SelectList(brand, "BrandId", "ProjectName");
            //ViewBag.ProductTypes = new SelectList(productTypes, "ProductTypeId", "Description");
            //ViewBag.ProductsList = new SelectList(product, "ProductId", "ProductName");
            //ViewBag.InterestedTags = interestTags ?? new List<InterestedTag>();

            ViewBag.Referral = new SelectList(referrals, "ReferralId", "Name");
            var leadViewModel = await _context.Leads.FindAsync(id);
            if (leadViewModel == null)
            {
                return NotFound();
            }


            return View(leadViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeadForm(int? leadId, Lead vm)
        {
            if (leadId == null)
            {
                return NotFound();
            }

            try
            {
                var existingLead = await _context.Leads.FindAsync(leadId);

                if (existingLead == null)
                {
                    return NotFound();
                }

                // Update existing lead properties from vm
                existingLead.FirstName = vm.FirstName;
                existingLead.LastName = vm.LastName;
                existingLead.Gender = vm.Gender;
                existingLead.CompanyName = vm.CompanyName;
                existingLead.JobTitle = vm.JobTitle;
                existingLead.Email = vm.Email;
                existingLead.PhoneNumber = vm.PhoneNumber;
                existingLead.Address = vm.Address;
                existingLead.City = vm.City;
                existingLead.State = vm.State;
                existingLead.ZipCode = vm.ZipCode;
                existingLead.Country = vm.Country;
                existingLead.Notes = vm.Notes;
                existingLead.Industry = vm.Industry;
                existingLead.LeadSource = vm.LeadSource;
                existingLead.LeadSourceDetails = vm.LeadSourceDetails;
                //existingLead.SelectedInterestedTags = vm.SelectedInterestedTags;
                //existingLead.BrandId = vm.BrandId;
                //existingLead.ProductTypeId = vm.ProductTypeId;
                //existingLead.ProductId = vm.ProductId;
                existingLead.ReferralId = vm.ReferralId;
                existingLead.IsLeadAssign = true;
 
                 _context.Update(existingLead);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadExists(leadId.Value))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            TempData["Success"] = "Lead data successfully edited.";
            return RedirectToAction("InProcessLead");
        }


        private bool LeadExists(int id)
        {
            return _context.Leads.Any(e => e.LeadId == id);
        }

        public async Task<IActionResult> CreateFollowUp(int id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null)
            {
                return NotFound();
            }

             ViewBag.FirstName = lead.FirstName;
            ViewBag.LeadId = id; // Set LeadId in ViewBag

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetFollowUpDetails(int leadId)
        {
            try
            {
                var followUp = await _context.FollowUps
                    .Where(f => f.LeadId == leadId)
                    .Select(f => new
                    {
                        Status =  f.Status,
                        ReminderDate = f.Reminder.HasValue ? f.Reminder.Value.ToString("yyyy-MM-dd") : null,
                        Description =    f.Description
                    })
                    .FirstOrDefaultAsync();

                if (followUp == null)
                {
                    return Json(new { success = false, message = "Follow-up not found" });
                }

                return Json(new { success = true, data = followUp });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(FollowUp model)
        {
            try
            {
                // Check if a FollowUp record with the given LeadId already exists
                var existingFollowUp = await _context.FollowUps
                    .FirstOrDefaultAsync(f => f.LeadId == model.LeadId);

                // Get the current user ID from session
                var user = await _userManager.GetUserAsync(User);
                var currentUserString = user?.Id;

                // If the current user ID is null, throw an exception
                if (string.IsNullOrEmpty(currentUserString))
                {
                    throw new ApplicationException("User not logged in");
                }

                // Create a new LeadHistory entry
                var leadHistory = new LeadHistory
                {
                    LeadId = model.LeadId,
                    UserId = currentUserString,
                    Status = model.Status,
                    LeadFollowupDate = DateTime.Now ,// Assuming FollowUpDate corresponds to LeadFollowupDate
              Description = model.Description  };

                if (existingFollowUp != null)
                {
                    // Update existing FollowUp record
                    existingFollowUp.EmployeeId = currentUserString; // Update EmployeeId with current user
                    existingFollowUp.FollowUpDate = model.FollowUpDate;
                    existingFollowUp.Status = model.Status;
                    existingFollowUp.Reminder = model.Reminder;
                    existingFollowUp.Description = model.Description;
                    existingFollowUp.FollowUpCompleted = model.FollowUpCompleted;
                    existingFollowUp.FollowUpDate = DateTime.Now;

                    _context.FollowUps.Update(existingFollowUp);

                    var reminderDate = model.Reminder;
                    var leadid = model.LeadId;
                    var description = model.Description;

                    if (reminderDate != null)
                    {
                        var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == leadid);

                        if (lead != null)
                        {
                            var fullName = lead.FirstName + " " + (lead.LastName ?? "");

                            var calendarEvent = await _context.CalendarEvents.FirstOrDefaultAsync(c => c.LeadId == leadid);

                            if (calendarEvent != null)
                            {
                                // Update existing CalendarEvent
                                calendarEvent.EventDate = reminderDate ?? DateTime.MinValue;
                                calendarEvent.Name = fullName;
                                calendarEvent.ThemeColor = "blue";
                                calendarEvent.Description = description;
                                calendarEvent.UserId = currentUserString;

                                _context.CalendarEvents.Update(calendarEvent);
                            }
                            else
                            {
                                // Create a new CalendarEvent
                                var cal = new CalendarEvent
                                {
                                    LeadId = leadid,
                                    EventDate = reminderDate ?? DateTime.MinValue, // Assign DateTime.MinValue if reminderDate is null
                                    Name = fullName,
                                    ThemeColor = "blue",
                                    UserId = currentUserString,
                                    Description = description
                                };

                                await _context.CalendarEvents.AddAsync(cal);
                            }
                        }
                    }
                }
                else
                {
                    // Create new FollowUp record
                    TempData["Success"] = "Follow-up created successfully";
                    model.FollowUpDate = DateTime.UtcNow;
                    model.EmployeeId = currentUserString; // Assign current user as the EmployeeId
                    await _context.FollowUps.AddAsync(model);

                    var reminderDate = model.Reminder;
                    var description = model.Description;
                    var leadid = model.LeadId;
                    var notifications = new Notification
                    {
                        UserId = currentUserString, // EmployeeId is saved as UserId
                        Message = model.Description,
                        NotificationTime = model.FollowUpDate,
                        IsRead = false
                    };

                    _context.Notifications.Add(notifications);
                    if (reminderDate != null)
                    {
                        var lead = await _context.Leads.FirstOrDefaultAsync(l => l.LeadId == leadid);

                        if (lead != null)
                        {
                            var fullName = lead.FirstName + " " + (lead.LastName ?? "");

                            var cal = new CalendarEvent
                            {
                                LeadId = leadid,
                                EventDate = reminderDate,
                                Name = fullName,
                                ThemeColor = "blue",
                                Description = description,
                                UserId = currentUserString

                            };

                            await _context.CalendarEvents.AddAsync(cal);
                        }
                    }
                }

                // Add LeadHistory record to the context
                _context.LeadHistories.Add(leadHistory);

                await _context.SaveChangesAsync();

                // Create a new ActivityLog entry
                var activityLog = new ActivityLog
                {
                    LeadId = model.LeadId,
                    UserId = currentUserString,
                    Status = model.Status,
                    Action = existingFollowUp != null ? "Follow-up updated" : "Follow-up created",
                    ActivityLogDate = DateTime.Now
                };

                // Add ActivityLog record to the context
                _context.ActivityLogs.Add(activityLog);

                await _context.SaveChangesAsync();

                // Return JSON success response
                return Json(new { success = true, message = "Follow-up saved successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }



        public async Task<IActionResult> DeadLead()
        {
             
            return View();
        }
        public async Task<JsonResult> GetDeadLeads()
        {
            // Replace the current user retrieval with session based approach
            
			var user = await _userManager.GetUserAsync(User);
			var currentUserString = user.Id;
			if (currentUserString == null)
            {
                return Json(new { data = new List<object>() });
            }

            var deadLeads = _context.FollowUps
                .Where(followUp => followUp.EmployeeId == currentUserString && followUp.Status == "Dead Lead")
                .Include(f => f.Lead)
                .Select(f => new
                {
                    f.Lead.LeadId,
                    f.Lead.FirstName,
                    f.Lead.LastName,
                    FullName = f.Lead.FirstName + " " + f.Lead.LastName,  

                    f.Lead.PhoneNumber,
                    f.Lead.Email,
                    ReferralName = f.Lead.Referral != null ? f.Lead.Referral.Name : string.Empty,
                    FollowUpStatus = f.Status
                })
                .ToList();

            return Json(new { data = deadLeads });
        }

        public async Task<IActionResult> ConvertedLead()
        {

            return View();
        }

        public async Task<JsonResult> GetConvertedLead()
        {
			// Replace the current user retrieval with session based approach
			var user = await _userManager.GetUserAsync(User);
			var currentUserString = user.Id;
			var Status = user.Applicationprocessor;
			if (currentUserString == null)
            {
                return Json(new { data = new List<object>() });
            }
            if (Status == false)
            {
                var convertedLeads = _context.FollowUps
                    .Where(followUp => followUp.EmployeeId == currentUserString && followUp.Status == "Converted Lead")
                    .Include(f => f.Lead)
                    .ThenInclude(l => l.Assignees)
                    .Select(f => new
                    {
                        f.Lead.LeadId,
                        f.Lead.FirstName,
                        f.Lead.LastName,
                        f.Lead.PhoneNumber,
                        f.Lead.Email,
                        f.Lead.Gender,
                        FullName = $"{f.Lead.FirstName} {f.Lead.LastName}",
                        IsEnrolled = f.Lead.IsEnrolled, // Include IsEnrolled property
                        ReferralName = Status,
                        EmployeeFullName = f.Lead.Assignees != null && f.Lead.Assignees.Any() && f.Lead.Assignees.First().Employee != null
                         ? $"{f.Lead.Assignees.First().Employee.FirstName} {f.Lead.Assignees.First().Employee.LastName}"
                         : string.Empty,
                        FollowUpStatus = f.Status,
                        
                    })
                    .ToList();

                return Json(new { data = convertedLeads });
            }
            else
            {
                var convertedLeads = _context.FollowUps
                    .Where(followUp =>  followUp.Status == "Converted Lead")
                    .Include(f => f.Lead)
                    .ThenInclude(l => l.Assignees)
                    .Select(f => new
                    {
                        f.Lead.LeadId,
                        f.Lead.FirstName,
                        f.Lead.LastName,
                        f.Lead.PhoneNumber,
                        f.Lead.Email,
                        f.Lead.Gender,

                        FullName = $"{f.Lead.FirstName} {f.Lead.LastName}",
                        IsEnrolled = f.Lead.IsEnrolled, // Include IsEnrolled property
                        ReferralName = Status,
                        EmployeeFullName = f.Lead.Assignees != null && f.Lead.Assignees.Any() && f.Lead.Assignees.First().Employee != null
                         ? $"{f.Lead.Assignees.First().Employee.FirstName} {f.Lead.Assignees.First().Employee.LastName}"
                         : string.Empty,
                        FollowUpStatus = f.Status
                    })
                    .ToList();
                return Json(new { data = convertedLeads });
            }
        }

        public async Task<IActionResult> UniversityProgram()
        {

            return View();
        }
        public async Task<IActionResult> Country()
        {

            return View();
        }
		public async Task<IActionResult> Programs()
		{

			return View();
		}
		public async Task<IActionResult> Followup()
        {

            return View();
        }
        public async Task<IActionResult> Status()
        {

            return View();
        }

        public async Task<IActionResult> Follow()
        {

            return View();
        }

        public async Task<IActionResult> FollowupStatus()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LeadForm(Lead vm)
        {
           
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			try
            {
                // Check if the email or phone number already exists in the database
                var existingLead = await _context.Leads
                    .FirstOrDefaultAsync(l => l.Email == vm.Email || l.PhoneNumber == vm.PhoneNumber);

                if (existingLead != null)
                {
                    return Json(new { success = true, message = "A lead with the same email or phone number already exists." });
                }

                // Create a new Lead object
                var lead = new Lead
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Gender = vm.Gender,
                    UserId = currentUser,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber,
                    CreatedDate = DateTime.UtcNow,
                    Industry = vm.Industry,
                    LeadSource = vm.LeadSource,
                    LeadSourceDetails = vm.LeadSourceDetails,
                    IsLeadAssign = true
                };

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                var leadId = lead.LeadId;

                // Create a new ActivityLog object
                var activityLog = new ActivityLog
                {
                    LeadId = leadId,
                    UserId = currentUser,
                    Status = "New Lead",
                    Action = "New Lead Added",
                    ActivityLogDate = DateTime.Now
                };

                // Add ActivityLog record to the context
                _context.ActivityLogs.Add(activityLog);

                // Create a new LeadAssignEmployee object
                var leadAssignEmployee = new LeadAssignEmployee
                {
                    EmployeeId = currentUser,
                    LeadId = leadId,
                    AssignmentDate = DateTime.Now
                };

                _context.LeadAssignEmployees.Add(leadAssignEmployee);
                await _context.SaveChangesAsync();

                // Return a success response
                return Json(new { success = true, message = "Lead saved successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Return an error response
                return Json(new { success = false, message = "An error occurred while saving the lead. Please try again.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLead(int id)
        {
            try
            {
                // Find the lead to delete
                var lead = await _context.Leads.FindAsync(id);

                if (lead == null)
                {
                    return NotFound(); // Lead not found
                }

                // Find and delete associated entries in LeadAssignEmployee
                var assignments = _context.LeadAssignEmployees.Where(a => a.LeadId == id);
                if (assignments.Any())
                {
                    _context.LeadAssignEmployees.RemoveRange(assignments);
                }

                // Find and delete associated entries in FollowUp
                var followUps = _context.FollowUps.Where(f => f.LeadId == id);
                if (followUps.Any())
                {
                    _context.FollowUps.RemoveRange(followUps);
                }

                // Delete from Leads
                _context.Leads.Remove(lead);

                await _context.SaveChangesAsync();

                // Return success response
                return Json(new { success = true, message = "Lead deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the lead. Please try again.", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditLead(Lead vm)
        {
            try
            {
                var lead = await _context.Leads.FindAsync(vm.LeadId);

                if (lead == null)
                {
                    return NotFound(); // Lead not found
                }

                lead.FirstName = vm.FirstName;
                lead.LastName = vm.LastName;
                lead.Email = vm.Email;
                lead.Gender = vm.Gender;
                lead.PhoneNumber = vm.PhoneNumber;
                lead.LeadSource = vm.LeadSource;
               

                _context.Leads.Update(lead);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Lead updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = "An error occurred while updating the lead. Please try again.", error = ex.Message });
            }
        }

        public async Task<JsonResult> GetAllUniversityData()
        {
            var universities = await _context.Universities
                .Include(u => u.UniversityCourses)
                    .ThenInclude(uc => uc.Course)
                .Select(u => new
                {
                    UniversityId = u.UniversityId,
                    Name = u.Name,
                    Country = u.Country,
                    Website = u.Website,  
                    Courses = u.UniversityCourses.Select(uc => new
                    {
                        UniversityCourseId = uc.UniversityCourseId,
                        CourseId = uc.CourseId,
                        CourseName = uc.Course.Name,
                        TuitionFee = uc.TuitionFee,
                        DurationInYears = uc.Course.DurationInYears,  
                        
                    }).ToList()  
                })
                .ToListAsync();

            return Json(new { data = universities });
        }
        public async Task<JsonResult> LeadByStatus(string status)
        {
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			var leads = await _context.Leads
                .Where(l => l.UserId == currentUser)
                .SelectMany(l => l.FollowUps.Where(f => f.Status == status).Select(f => new
                {
                    l.FirstName,
                    l.LastName,
                    FullName = l.FirstName +  " "+  l.LastName,
                    l.Email,
                    l.Gender,
                    l.PhoneNumber,
                    Status = f.Status,
                    LeadId = l.LeadId
                }))
                .ToListAsync();

            return Json(new { data = leads });
        }
    }
}



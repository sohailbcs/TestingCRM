using CsvHelper;
using LegalAndGeneralConsultantCRM.Areas.Admin.SendNotificationHub;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.ViewModels;
using LegalAndGeneralConsultantCRM.ViewModels.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class LeadController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public LeadController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        [HttpPost]
        public IActionResult DeleteLeads([FromBody] DeleteLeadsRequest request)
        {
            if (request.LeadIds == null || request.LeadIds.Count == 0)
            {
                return Json(new { success = false, message = "No leads selected for deletion." });
            }

            try
            {
                // Fetch the leads to be deleted from the database
                var leadsToDelete = _context.Leads.Where(lead => request.LeadIds.Contains(lead.LeadId)).ToList();

                if (leadsToDelete.Count == 0)
                {
                    return Json(new { success = false, message = "No matching leads found for deletion." });
                }

                // Remove each lead from the context
                _context.Leads.RemoveRange(leadsToDelete);

                // Save changes to the database
                _context.SaveChanges();

                return Json(new { success = true, message = "Leads deleted successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public class DeleteLeadsRequest
        {
            public List<int> LeadIds { get; set; }
        }

        // Action method to get lead history by lead ID
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
        public IActionResult GetDomains(int universityId)
        {
            var domains = _context.Domain
                .Where(d => d.UniversityID == universityId)
                .Select(d => new { d.DomainId, d.DomainNme }) // Ensure property name matches your model
                .ToList();

            return Json(domains);
        }

  
        public IActionResult GetPrograms(int domainId)
        {
            var programs = _context.UniversityCourses
                .Where(uc => uc.DomainId == domainId)
                .Select(uc => new { uc.CourseId, CourseName = uc.Course.Name }) // Assuming you have a Course navigation property
                .ToList();

            return Json(programs);
        }


        public async Task<IActionResult> FollowUpDetails(string status)
        {
            var followUps = await _context.FollowUps
                .Include(f => f.Lead)
                .Include(f => f.Employee)
                .Where(f => status == null || f.Status == status)
                .Select(f => new FollowUpViewModel
                {
                    EmployeeFullName = f.Employee != null ? $"{f.Employee.FirstName} {f.Employee.LastName}" : "N/A",
                    LeadEmail = f.Lead.Email,
                    LeadFullName = $"{f.Lead.FirstName} {f.Lead.LastName}",
                    PhoneNumber = f.Lead.PhoneNumber,
                    Status = f.Status,
                    FollowUpDate = f.FollowUpDate
                })
                .ToListAsync();

            return View(followUps);
        }

        public async Task<IActionResult> LeadForm()
        {
            // Fetch the referrals list
            var referrals = await _context.Referrals.ToListAsync();
            ViewBag.Referral = new SelectList(referrals, "ReferralId", "Name");

            // Fetch the list of universities
            var universities = await _context.Universities.ToListAsync();
            ViewBag.Universities = new SelectList(universities, "UniversityId", "Name");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LeadForm(Lead vm)
        {
            // Check if a lead with the same email or phone number already exists
            var existingLead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Email == vm.Email || l.PhoneNumber == vm.PhoneNumber);

            if (existingLead != null)
            {

                TempData["error"] = "A lead with the same email or phone number already exists.";
                return View("LeadForm"); // Return the view with the existing model to display errors
            }

            // If no existing lead is found, proceed with adding the new lead
            var lead = new Lead
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Gender = vm.Gender,
                CompanyName = vm.CompanyName,
                JobTitle = vm.JobTitle,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                CreatedDate = DateTime.Now,
                Address = vm.Address,
                City = vm.City,
                State = vm.State,
                ZipCode = vm.ZipCode,
                Country = vm.Country,
                Notes = vm.Notes,
                Industry = vm.Industry,
                LeadSource = vm.LeadSource,
                LeadSourceDetails = vm.LeadSourceDetails,
                UniversityId = vm.UniversityId,
                DomainId = vm.DomainId,
                interestedcountry = vm.interestedcountry,
                interestedprogram = vm.interestedprogram,
                yearCompletion = vm.yearCompletion,
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            // Set a success message in TempData
            TempData["SuccessMessage"] = "Lead saved successfully.";

            return RedirectToAction("AllLead");
        }

        [HttpPost]
        public async Task<IActionResult> EditLead([FromBody] Lead lead)
        {
            try
            {
                // Update lead in the database
                var existingLead = await _context.Leads.FindAsync(lead.LeadId);
                if (existingLead != null)
                {
                    existingLead.FirstName = lead.FirstName;
                    existingLead.LastName = lead.LastName;
                    existingLead.PhoneNumber = lead.PhoneNumber;
                    existingLead.Email = lead.Email;
                    existingLead.LeadSource = lead.LeadSource;
                    existingLead.Address = lead.Address;
                    existingLead.Gender = lead.Gender;

                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Lead updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Lead not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating lead: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLead([FromBody] DeleteLeadRequest request)
        {
            try
            {
                var existingLead = await _context.Leads.FindAsync(request.LeadId);
                if (existingLead != null)
                {
                    _context.Leads.Remove(existingLead);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Lead deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Lead not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting lead: " + ex.Message });
            }
        }

        public class DeleteLeadRequest
        {
            public int LeadId { get; set; }
        }

        public IActionResult BulkUpload()
        {
            return View();
        }
        public IActionResult ShowResult(int totalRecords, int savedRowCount, int duplicateCount)
        {
            var model = new ResultViewModel
            {
                TotalRecords = totalRecords,
                SavedRowCount = savedRowCount,
                DuplicateRecords = duplicateCount
            };

            return View(model);
        }

        // POST: /Admin/Lead/UploadBulkData

        [HttpPost]
        public async Task<IActionResult> UploadEmployeeExcel(IFormCollection form, string leadSource)
        {
            var mappedColumns = JsonConvert.DeserializeObject<List<MappedColumn>>(form["mappedColumns"]);
            var fileData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(form["fileData"]);
            int totalRecords = fileData.Count;
            int savedRowCount = 0;
            int duplicateCount = 0;

            // Get the current user ID from session
            var user = await _userManager.GetUserAsync(User);
            var currentUserString = user?.Id;

            // If the current user ID is null, throw an exception
            if (string.IsNullOrEmpty(currentUserString))
            {
                throw new ApplicationException("User not logged in");
            }
            if (fileData.Count > 0 && fileData[0].ContainsKey("id"))
            {
                // Process the file with "id" in the first column
                foreach (var row in fileData)
                {
                    var employee = new Lead();
                    bool isValid = true; // Flag to track if the row is valid

                    if (row.ContainsKey("id"))
                    {
                        var id = row["id"]?.ToString();

                        if (!string.IsNullOrEmpty(id))
                        {
                            // Check if this id already exists in the database
                            var existingEmployee = _context.Leads
                                .FirstOrDefault(e => e.SourceId == id);

                            if (existingEmployee != null)
                            {
                                duplicateCount++;
                                // Log and skip this record
                                Console.WriteLine($"ID {id} already exists. Skipping this entry.");
                                continue; // Skip to the next row
                            }
                            else
                            {
                                // Assign the id to the Source column
                                employee.SourceId = id;
                                employee.LeadSource = leadSource;
                                employee.UserId = currentUserString;

                            }
                        }
                    }

                    // Process the rest of the columns as usual
                    foreach (var mappedColumn in mappedColumns)
                    {
                        if (row.ContainsKey(mappedColumn.FileColumn))
                        {
                            switch (mappedColumn.DatabaseColumn)
                            {
                                case "FirstName":
                                    employee.FirstName = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "LastName":
                                    employee.LastName = row[mappedColumn.FileColumn]?.ToString();
                                    break;

                                case "PhoneNumber":
                                    employee.PhoneNumber = row[mappedColumn.FileColumn]?.ToString();
                                    break; // Exit switch-case for this row


                                // Check if this phone number already exists in the database





                                case "Gender":
                                    employee.Gender = row[mappedColumn.FileColumn]?.ToString();
                                    break;

                                case "Eaducation":
                                    employee.Eaducation = row[mappedColumn.FileColumn]?.ToString();
                                    break;

                                case "PhoneNumber2":
                                    employee.PhoneNumber2 = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "CompanyName":
                                    employee.CompanyName = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "JobTitle":
                                    employee.JobTitle = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Email":
                                    employee.Email = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "CreatedDate":
                                    if (DateTime.TryParse(row[mappedColumn.FileColumn]?.ToString(), out DateTime createdDate))
                                    {
                                        employee.CreatedDate = createdDate;
                                    }
                                    break;
                                case "Address":
                                    employee.Address = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "City":
                                    employee.City = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "State":
                                    employee.State = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "ZipCode":
                                    employee.ZipCode = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Country":
                                    employee.Country = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Notes":
                                    employee.Notes = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Industry":
                                    employee.Industry = row[mappedColumn.FileColumn]?.ToString();
                                    break;

                                case "LeadSourceDetails":
                                    employee.LeadSourceDetails = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "IsLeadAssign":
                                    employee.IsLeadAssign = row[mappedColumn.FileColumn]?.ToString()?.ToLower() == "true";
                                    break;
                                case "IsEnrolled":
                                    employee.IsEnrolled = row[mappedColumn.FileColumn]?.ToString()?.ToLower() == "true";
                                    break;
                             
                               


                            }
                        }
                    }

                    if (isValid)
                    {
                        employee.CreatedDate = DateTime.Now;
                        _context.Leads.Add(employee);
                        savedRowCount++; // Increment the saved row count

                    }
                }
            }
            else
            {
                // Process normally if the first column is not "id"
                foreach (var row in fileData)
                {
                    var employee = new Lead();
                    bool isValid = true; // Flag to track if the row is valid

                    foreach (var mappedColumn in mappedColumns)
                    {
                        if (row.ContainsKey(mappedColumn.FileColumn))
                        {
                            switch (mappedColumn.DatabaseColumn)
                            {
                                case "FirstName":
                                    employee.FirstName = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "LastName":
                                    employee.LastName = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Eaducation":
                                    employee.Eaducation = row[mappedColumn.FileColumn]?.ToString();
                                    break;

                                case "PhoneNumber2":
                                    employee.PhoneNumber2 = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "PhoneNumber":
                                    var phoneNumber = row[mappedColumn.FileColumn]?.ToString();
                                    if (string.IsNullOrEmpty(phoneNumber))
                                    {
                                        duplicateCount++;
                                        // Log and skip this record if phone number is empty
                                        Console.WriteLine("Phone number is empty. Skipping this entry.");
                                        isValid = false;
                                        break; // Exit switch-case for this row
                                    }

                                    // Check if this phone number already exists in the database
                                    var existingPhoneEmployee = _context.Leads
                                        .FirstOrDefault(e => e.PhoneNumber == phoneNumber);

                                    if (existingPhoneEmployee != null)
                                    {
                                        // Log and skip this record
                                        Console.WriteLine($"Phone number {phoneNumber} already exists. Skipping this entry.");
                                        isValid = false;
                                        break; // Exit switch-case for this row
                                    }
                                    else
                                    {
                                        employee.PhoneNumber = phoneNumber; // Assign the phone number to the model if it's unique
                                    }
                                    break; // Exit switch-case for this row

                                case "Gender":
                                    employee.Gender = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "CompanyName":
                                    employee.CompanyName = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "JobTitle":
                                    employee.JobTitle = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Email":
                                    employee.Email = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "CreatedDate":
                                    if (DateTime.TryParse(row[mappedColumn.FileColumn]?.ToString(), out DateTime createdDate))
                                    {
                                        employee.CreatedDate = createdDate;
                                    }
                                    break;
                                case "Address":
                                    employee.Address = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "City":
                                    employee.City = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "State":
                                    employee.State = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "ZipCode":
                                    employee.ZipCode = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Country":
                                    employee.Country = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Notes":
                                    employee.Notes = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "Industry":
                                    employee.Industry = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "LeadSource":
                                    employee.LeadSource = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "LeadSourceDetails":
                                    employee.LeadSourceDetails = row[mappedColumn.FileColumn]?.ToString();
                                    break;
                                case "IsLeadAssign":
                                    employee.IsLeadAssign = row[mappedColumn.FileColumn]?.ToString()?.ToLower() == "true";
                                    break;
                                case "IsEnrolled":
                                    employee.IsEnrolled = row[mappedColumn.FileColumn]?.ToString()?.ToLower() == "true";
                                    break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        employee.CreatedDate = DateTime.Now;
                        employee.LeadSource = leadSource;
                        _context.Leads.Add(employee);
                        savedRowCount++; // Increment the saved row count

                    }
                }
            }

            await _context.SaveChangesAsync();

            // Redirect to ShowResult action with query parameters
            return RedirectToAction("ShowResult", new
            {
                totalRecords,
                savedRowCount,
                duplicateCount,
            });
        }
        public class MappedColumn
        {
            public string FileColumn { get; set; }
            public string DatabaseColumn { get; set; }
        }
        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }

        private bool IsValidGender(string gender)
        {
            var validGenders = new[] { "male", "female" };
            return validGenders.Contains(gender?.ToLower());
        }

        private bool LeadExists(string email, string phone)
        {
            return _context.Leads.Any(l => l.Email == email || l.PhoneNumber == phone);
        }



        public async Task<IActionResult> AssignLeadToEmployee()
        {
            // Get leads with IsSelected == null
            var leads = await _context.Leads.Where(l => l.IsLeadAssign == null || l.IsLeadAssign == false).ToListAsync();

            // Get users with the role "Employee"
            var users = await _userManager.GetUsersInRoleAsync("Employee");

            ViewBag.Leads = leads;

            ViewBag.UserList = new SelectList(users.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}"
            }), "Id", "FullName");

            return View(new LeadEmployeeAssignmentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AssignLeadToEmployee(string Id, List<int> leadIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int numberOfLeadsAssigned = 0;

                    foreach (var leadId in leadIds)
                    {
                        var lead = await _context.Leads.FindAsync(leadId);

                        if (lead != null)
                        {
                            lead.IsLeadAssign = true;
                            lead.UserId = Id;
                            var leadAssignEmployee = new LeadAssignEmployee
                            {
                                EmployeeId = Id,
                                LeadId = leadId,
                                AssignmentDate = DateTime.Now
                            };

                            _context.LeadAssignEmployees.Add(leadAssignEmployee);
                            numberOfLeadsAssigned++;
                            var notifications = new Notification
                            {
                                UserId = Id, // EmployeeId is saved as UserId
                                Message = $"A new lead  has been assigned to you from Admin.",
                                NotificationTime = DateTime.Now,
                                IsRead = false
                            };

                            _context.Notifications.Add(notifications);
                        }
                    }

                    _context.Leads.UpdateRange(await _context.Leads.Where(l => leadIds.Contains(l.LeadId)).ToListAsync());
                    await _context.SaveChangesAsync();

                    var assignedEmployee = await _userManager.FindByIdAsync(Id.ToString());
                  
                    await _userManager.UpdateAsync(assignedEmployee);

                    // Check if a notification already exists for the user


                    // Send the message to the specific user
                    await _hubContext.Clients.User(assignedEmployee.Id).SendAsync("ReceiveNotification", assignedEmployee.Notification);

                    // Set a success message in TempData
                    TempData["SuccessMessage"] = $"{numberOfLeadsAssigned} lead{(numberOfLeadsAssigned > 1 ? "s" : "")} assigned successfully.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AssignLeadToEmployee: {ex.Message}");
            }

            return RedirectToAction("AssignLeadToEmployee");
        }


        [HttpGet]
        public IActionResult GetContactInfo(int referralId)
        {
            // Fetch the Referral object based on the referralId
            var referral = _context.Referrals.FirstOrDefault(r => r.ReferralId == referralId);

            return Json(new { Contact1 = referral.Contact1 });

        }


        public async Task<IActionResult> AllLead()
        {

            return View();
        }


     public async Task<JsonResult> GetAllLeadData()
{
    var leadData = await _context.Leads
        .Include(l => l.Referral)
        .Include(l => l.University) // Assuming this is a related entity
        .Include(l => l.Domain) // Assuming this is a related entity
        .Include(l => l.UniversityCourse) // Adjusted to pluralize correctly
            .ThenInclude(uc => uc.Course) // Ensure Course is correctly related
        .Select(l => new
        {
            l.LeadId,
            l.FirstName,
            l.LastName,
            l.PhoneNumber,
            FullName = l.FirstName + " " + l.LastName,
            l.Email,
            l.Address,
            l.Gender,
            l.interestedcountry,
            l.interestedprogram,
            UniversityName = l.University != null ? l.University.Name : null,
            DomainName = l.Domain != null ? l.Domain.DomainNme : null, // Ensure DomainNme is correct
            l.UniversityCourse,
            l.Eaducation,
            l.yearCompletion,
            l.LeadSource,
            l.IsLeadAssign,
            CreatedDate = l.CreatedDate.HasValue ? l.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
        })
        .ToListAsync();

    return Json(new { data = leadData });
}



        public IActionResult ExportToCSV()
        {
            var leads = _context.Leads
                .Include(l => l.Referral)
                .ToList();

            var leadData = leads.Select(l => new
            {
                l.FirstName,
                l.LastName,
                l.PhoneNumber,
                l.Email,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd"), // Format date as needed
                ReferralName = l.Referral != null ? l.Referral.Name : ""
            }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("First Name,Last Name,Phone Number,Email,Creation Date,Referral Name");

            foreach (var lead in leadData)
            {
                sb.AppendLine($"{Escape(lead.FirstName)},{Escape(lead.LastName)},{Escape(lead.PhoneNumber)},{Escape(lead.Email)},{lead.CreatedDate},{Escape(lead.ReferralName)}");
            }

            byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

            return File(result, "text/csv", "leads.csv");
        }

        private string Escape(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            string text = obj.ToString();

            // Escape double quotes with double quotes
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }
        public IActionResult ExportToExcel()
        {
            var leads = _context.Leads
                .Include(l => l.Referral)
                .ToList();

            var leadData = leads.Select(l => new
            {
                l.FirstName,
                l.LastName,
                l.PhoneNumber,
                l.Email,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd"), // Format date as needed
            }).ToList();

            byte[] result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Leads");

                // Header row
                worksheet.Cells["A1"].Value = "First Name";
                worksheet.Cells["B1"].Value = "Last Name";
                worksheet.Cells["C1"].Value = "Phone Number";
                worksheet.Cells["D1"].Value = "Email";
                worksheet.Cells["E1"].Value = "Creation Date";

                // Data rows
                int row = 2;
                foreach (var lead in leadData)
                {
                    worksheet.Cells[row, 1].Value = lead.FirstName;
                    worksheet.Cells[row, 2].Value = lead.LastName;
                    worksheet.Cells[row, 3].Value = lead.PhoneNumber;
                    worksheet.Cells[row, 4].Value = lead.Email;
                    worksheet.Cells[row, 5].Value = lead.CreatedDate;
                    row++;
                }

                result = package.GetAsByteArray();
            }

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "leads.xlsx");
        }

        public async Task<IActionResult> AllocatedLead()
        {

            return View();
        }
        public JsonResult GetAllocatedLeadData()
        {
            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true)
                .Include(l => l.Referral)
                .Include(l => l.Assignees)
                    .ThenInclude(lae => lae.Employee).Include(l => l.FollowUps)
                .ToList();

            var leadData = allocatedLeads.Select(l => new
            {
                l.FirstName,
                l.LastName,
                l.PhoneNumber,
                CreatedDate = l.CreatedDate?.Date,
                ReferralName = l.Referral != null ? l.Referral.Name : null,
                EmployeeFullName = l.Assignees?.FirstOrDefault()?.Employee?.FirstName + " " + l.Assignees?.FirstOrDefault()?.Employee?.LastName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status,
                FullName = $"{l.FirstName} {l.LastName}"

            });

            return Json(new { data = leadData });
        }

        public IActionResult ExportAllocatedLeadsToExcel()
        {
            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true)
                .Include(l => l.Referral)
                .Include(l => l.Assignees).ThenInclude(lae => lae.Employee)
                .Include(l => l.FollowUps)
                .ToList();

            var leadData = allocatedLeads.Select(l => new
            {
                l.FirstName,
                l.LastName,
                l.PhoneNumber,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd"), // Format date as needed
                ReferralName = l.Referral != null ? l.Referral.Name : "",
                EmployeeFullName = l.Assignees?.FirstOrDefault()?.Employee?.FirstName + " " + l.Assignees?.FirstOrDefault()?.Employee?.LastName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status
            }).ToList();

            byte[] result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Allocated Leads");

                // Header row
                worksheet.Cells["A1"].Value = "First Name";
                worksheet.Cells["B1"].Value = "Last Name";
                worksheet.Cells["C1"].Value = "Phone Number";
                worksheet.Cells["D1"].Value = "Creation Date";
                worksheet.Cells["F1"].Value = "Assigned Employee";
                worksheet.Cells["G1"].Value = "Follow Up Status";

                // Data rows
                int row = 2;
                foreach (var lead in leadData)
                {
                    worksheet.Cells[row, 1].Value = lead.FirstName;
                    worksheet.Cells[row, 2].Value = lead.LastName;
                    worksheet.Cells[row, 3].Value = lead.PhoneNumber;
                    worksheet.Cells[row, 4].Value = lead.CreatedDate;
                    worksheet.Cells[row, 6].Value = lead.EmployeeFullName;
                    worksheet.Cells[row, 7].Value = lead.FollowUpStatus;
                    row++;
                }

                result = package.GetAsByteArray();
            }

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "allocated_leads.xlsx");
        }
        public IActionResult ExportAllocatedLeadsToCSV()
        {
            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true)
                .Include(l => l.Assignees).ThenInclude(lae => lae.Employee)
                .Include(l => l.FollowUps)
                .ToList();

            var leadData = allocatedLeads.Select(l => new
            {
                l.FirstName,
                l.LastName,
                l.PhoneNumber,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd"), // Format date as needed
                EmployeeFullName = l.Assignees?.FirstOrDefault()?.Employee?.FirstName + " " + l.Assignees?.FirstOrDefault()?.Employee?.LastName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status
            }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("First Name,Last Name,Phone Number,Creation Date,Assigned Employee,Follow Up Status");

            foreach (var lead in leadData)
            {
                sb.AppendLine($"{Escape(lead.FirstName)},{Escape(lead.LastName)},{Escape(lead.PhoneNumber)},{lead.CreatedDate},{Escape(lead.EmployeeFullName)},{lead.FollowUpStatus}");
            }

            byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

            return File(result, "text/csv", "allocated_leads.csv");
        }

        public async Task<IActionResult> UnAllocatedLead()
        {

            return View();
        }
        public JsonResult GetUnAllocatedLeadData()
        {
            var allocatedLeads = _context.Leads
              .Where(lead => lead.IsLeadAssign == false)
              .Include(l => l.Referral)
              .ToList();

            var leadData = allocatedLeads.Select(l => new
            {
                l.FirstName,
                l.PhoneNumber,
                l.Email,
                CreatedDate = l.CreatedDate?.Date,
                ReferralName = l.Referral != null ? l.Referral.Name : null,
            });

            return Json(new { data = leadData });

        }

        public IActionResult ExportUnAllocatedLeadsToCSV()
        {
            var unallocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == false)
                .ToList();

            var leadData = unallocatedLeads.Select(l => new
            {
                l.FirstName,
                l.PhoneNumber,
                l.Email,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd") // Format date as needed
            }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("First Name,Phone Number,Email,Creation Date");

            foreach (var lead in leadData)
            {
                sb.AppendLine($"{Escape(lead.FirstName)},{Escape(lead.PhoneNumber)},{Escape(lead.Email)},{lead.CreatedDate}");
            }

            byte[] result = Encoding.UTF8.GetBytes(sb.ToString());

            return File(result, "text/csv", "unallocated_leads.csv");
        }
        public IActionResult ExportUnAllocatedLeadsToExcel()
        {
            var unallocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == false)
                .ToList();

            var leadData = unallocatedLeads.Select(l => new
            {
                l.FirstName,
                l.PhoneNumber,
                l.Email,
                CreatedDate = l.CreatedDate?.ToString("yyyy-MM-dd") // Format date as needed
            }).ToList();

            byte[] result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Unallocated Leads");

                // Header row
                worksheet.Cells["A1"].Value = "First Name";
                worksheet.Cells["B1"].Value = "Phone Number";
                worksheet.Cells["C1"].Value = "Email";
                worksheet.Cells["D1"].Value = "Creation Date";

                // Data rows
                int row = 2;
                foreach (var lead in leadData)
                {
                    worksheet.Cells[row, 1].Value = lead.FirstName;
                    worksheet.Cells[row, 2].Value = lead.PhoneNumber;
                    worksheet.Cells[row, 3].Value = lead.Email;
                    worksheet.Cells[row, 4].Value = lead.CreatedDate;
                    row++;
                }

                result = package.GetAsByteArray();
            }

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "unallocated_leads.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> LeadHistory(int leadId)
        {
            if (leadId <= 0)
            {
                return NotFound();
            }

            var leadHistory = await _context.LeadHistories
                .Include(lh => lh.User) // Include related user details if needed
                .Where(lh => lh.LeadId == leadId)
                .Select(lh => new LeadHistoryViewModel
                {
                    FullName = $"{lh.Lead.FirstName} {lh.Lead.LastName}",
                    PhoneNumber = lh.Lead.PhoneNumber,
                    Email = lh.Lead.Email,
                    Status = lh.Status,
                    TeamMember = lh.User.FirstName,
                    LastFollowUps = lh.LeadFollowupDate,
                    Description = lh.Description,
                })
                .ToListAsync();

            if (leadHistory == null)
            {
                return NotFound();
            }

            return View(leadHistory);
        }

        public async Task<JsonResult> GetLeadDetails(int leadId)
        {
            var leadHistoryData = await _context.LeadHistories
                .Include(lh => lh.Lead)
                .Include(lh => lh.User)
                .Where(lh => lh.LeadId == leadId)
                .Select(lh => new
                {
                    LeadFullName = lh.Lead.FirstName + " " + lh.Lead.LastName,
                    Email = lh.Lead.Email,
                    PhoneNumber = lh.Lead.PhoneNumber,
                    Status = lh.Status,
                    SLeadFollowupDatetatus = lh.LeadFollowupDate,
                    Username = lh.User.FirstName + " " + lh.User.LastName // Assuming UserName is what you want to return
                })
                .ToListAsync();

            return Json(new { data = leadHistoryData });
        }
        public async Task<IActionResult> DeAssign()
        {

            return View();
        }
        public JsonResult Leads()
        {
            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true)
                .Include(l => l.Referral)
                .Include(l => l.Assignees)
                    .ThenInclude(lae => lae.Employee)
                .Include(l => l.FollowUps)
                .ToList();

            var leadData = allocatedLeads
                .Where(l => l.FollowUps.FirstOrDefault()?.Status != "Converted Lead") // Filter out converted leads
                .Select(l => new
                {
                    l.LeadId,
                    l.FirstName,
                    l.LastName,
                    l.PhoneNumber,
                    CreatedDate = l.CreatedDate?.Date,
                    ReferralName = l.Referral != null ? l.Referral.Name : null,
                    EmployeeFullName = l.Assignees?.FirstOrDefault()?.Employee?.FirstName + " " + l.Assignees?.FirstOrDefault()?.Employee?.LastName,
                    FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status,
                    FullName = $"{l.FirstName} {l.LastName}"
                });

            return Json(new { data = leadData });
        }
        [HttpPost]
        public async Task<IActionResult> DesignLead([FromBody] int[] leadIds) // Accepts array of leadIds
        {
            if (leadIds == null || leadIds.Length == 0)
            {
                return Json(new { success = false, message = "No lead selected." });
            }

            var leads = await _context.Leads
                .Where(l => leadIds.Contains(l.LeadId))
                .ToListAsync();

            if (leads == null || !leads.Any())
            {
                return Json(new { success = false, message = "No leads found." });
            }

            foreach (var lead in leads)
            {
                // Perform the design action on each lead
                // For example:
                lead.IsLeadAssign = false;  // Example field to track if lead is designed
                lead.IsEnrolled = false;  // Example field to track if lead is designed
                _context.Leads.Update(lead);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Leads designed successfully." });
        }


    }
}

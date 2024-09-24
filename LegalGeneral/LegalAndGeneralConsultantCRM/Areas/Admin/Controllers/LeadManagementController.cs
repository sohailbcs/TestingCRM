using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using LegalAndGeneralConsultantCRM.Models.Leads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class LeadManagementController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;

        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

        public LeadManagementController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("Employee");
            ViewBag.UserList = new SelectList(users, "Id", "FirstName");

            return View();
        }

        public JsonResult GetLeadManageData()
        {
            var brands = _context.FollowUps.ToList();

            return Json(new { data = brands });
        }

        [HttpGet]
        public IActionResult GetLeadDataByEmployeeId(string employeeId)
        {
            // Perform a query to retrieve lead data based on the provided employeeId
            var leadIds = _context.LeadAssignEmployees
                .Where(assign => assign.EmployeeId == employeeId)
                .Select(assign => assign.LeadId)
                .ToList();

            // Set ViewBag.SelectedEmployeeId to the current employeeId
            ViewBag.SelectedEmployeeId = employeeId;

            // Retrieve all leads associated with the leadIds
            var leadData = _context.Leads
                .Where(lead => leadIds.Contains(lead.LeadId))
                .ToList();

            return PartialView("_LeadDataPartialView", leadData);
        }


        public IActionResult ManageLead(int leadId, string employeeId)
        {
            var lead = _context.Leads.FirstOrDefault(l => l.LeadId == leadId);

            if (lead == null)
            {
                return NotFound();
            }

            // Retrieve the employee information based on employeeId from the CRMUser table
            var employee = _context.Users.FirstOrDefault(u => u.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            var followUp = new FollowUp
            {
                LeadId = lead.LeadId,
                EmployeeId = employee.Id,
                FollowUpDate = DateTime.Now, // Set to current date and time as an example
                Status = "", // Set to a default status if needed
                Reminder = DateTime.Now.AddDays(7), // Set a default reminder date (7 days from now)
                Description = "", // Set a default description if needed
                FollowUpCompleted = false // Set to false as the default
            };

            // Pass the followUp instance to the view
            ViewBag.EmployeeName = employee;
            ViewBag.EmployeeId = employee.Id;
            return View(followUp);
        }


        [HttpPost]
        public async Task<IActionResult> ManageLead(FollowUp model)
        {
            try
            {
                var existingFollowUp = await _context.FollowUps
                    .FirstOrDefaultAsync(f => f.LeadId == model.LeadId);

                if (existingFollowUp != null)
                {
                    // Update existing follow-up with properties from the model
                    existingFollowUp.EmployeeId = model.EmployeeId;
                    existingFollowUp.FollowUpDate = model.FollowUpDate;
                    existingFollowUp.Status = model.Status;
                    existingFollowUp.Reminder = model.Reminder;
                    existingFollowUp.Description = model.Description;
                    existingFollowUp.FollowUpCompleted = model.FollowUpCompleted;
                }
                else
                {
                    // Add new follow-up
                    var newFollowUp = new FollowUp
                    {
                        EmployeeId = model.EmployeeId,
                        LeadId = model.LeadId,
                        FollowUpDate = model.FollowUpDate,
                        Status = model.Status,
                        Reminder = model.Reminder,
                        Description = model.Description,
                        FollowUpCompleted = model.FollowUpCompleted
                    };

                    _context.FollowUps.Add(newFollowUp);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "LeadManagement", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                // For simplicity, you can redirect to an error view
                return View("Error");
            }
        }

        public async Task<IActionResult> LeadEdit(int id)
        {
            
            var referrals = await _context.Referrals.ToListAsync();

           

            ViewBag.Referral = new SelectList(referrals, "ReferralId", "Name");
            var leadViewModel = await _context.Leads.FindAsync(id);
            if (leadViewModel == null)
            {
                return NotFound();
            }

 
            return View(leadViewModel);
        }
        
        public async Task<IActionResult> EditLeadForm(int id)
        {
             
            var referrals = await _context.Referrals.ToListAsync();
 

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
               
                existingLead.ReferralId = vm.ReferralId;
                existingLead.IsLeadAssign = true;
                // Update other properties...

                // Update the existing lead
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

            return RedirectToAction("Index");
        }

        private bool LeadExists(int id)
        {
            return _context.Leads.Any(e => e.LeadId == id);
        }
    }
}

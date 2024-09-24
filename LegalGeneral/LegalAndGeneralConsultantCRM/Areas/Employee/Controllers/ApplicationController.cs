using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using LegalAndGeneralConsultantCRM.ViewModels.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class ApplicationController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        
        private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public ApplicationController(LegalAndGeneralConsultantCRMContext context, IWebHostEnvironment webHostEnvironment, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
		}

		public IActionResult Index()
        {
            return View();
        }
        public IActionResult Form()
        {
            return View();
        }
        public IActionResult ApplicationStatus()
        {
            return View();
        }
       
        public IActionResult DepositPay()
        {
            return View();
        }
        public IActionResult VisaApplication()
        {
            return View();
        }
        public IActionResult VisaStatus()
        {
            return View();
        }
        public IActionResult Enrolled()
        {
            return View();
        }
		public IActionResult Documents()
		{
			return View();
		}
        [HttpGet]
        public IActionResult GetDepositByLeadId(int leadId)
        {
            var deposit = _context.Deposits
                .Include(d => d.Lead)
                .Where(d => d.LeadId == leadId)
                .Select(d => new
                {
                    d.DepositId,
                    d.AccountNumber,
                    d.Amount,
                    d.DepositDate,
                    d.DepositPayImagePath,
                    d.AccountTitle
                })
                .FirstOrDefault();

            if (deposit == null)
            {
                return Json(new { success = false, message = "No deposit details found for the specified lead." });
            }

            return Json(new { success = true, data = deposit });
        }

        public async Task<JsonResult> LeadConvertedStatus()
        {
            
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			// Query to find leads where UserId matches currentUser
			var leads = await _context.Leads
                .Include(l => l.FollowUps) // Include FollowUps for each lead
                .Where(l => l.UserId == currentUser) // Filter by current user
                .ToListAsync();
            var deposit = _context.Deposits
                                        .Where(d => d.UserId == currentUser)
                                        .FirstOrDefault();

            string accountTitle = deposit != null ? deposit.AccountTitle : null;
            // Filter for follow-ups with status "Converted Lead" and retrieve associated messages
            var convertedLeads = leads
                .Where(l => l.FollowUps.Any(f => f.Status == "Converted Lead"))
                .Select(l => new
                {
                    LeadId = l.LeadId,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    PhoneNumber = l.PhoneNumber,
                    Email = l.Email,
                    AccountTitle = accountTitle,  
                    Gender = l.Gender,  
                    FullName = l.FirstName + " " +l.LastName,
                    UniversityName = _context.Students
                                 .Where(s => s.LeadId == l.LeadId)
                                 .Include(s => s.University)
                                 .Select(s => s.University.Name)
                                 .FirstOrDefault(),
                    CourseName = _context.Students
                             .Where(s => s.LeadId == l.LeadId)
                             .Include(s => s.Course)
                             .Select(s => s.Course.Name)
                             .FirstOrDefault(), // Course Name

                    Messages = string.Join("<br/>", _context.StudentMessages
                        .Where(sm => sm.LeadId == l.LeadId)
                        .Select(sm => sm.Message)
                        .ToArray()),
                    Statuses = string.Join("<br/>", _context.StudentMessages
                        .Where(sm => sm.LeadId == l.LeadId)
                        .Select(sm => sm.Status)
                        .ToArray())
                })
                .ToList();

            return Json(new { data = convertedLeads });
        }

        public async Task<JsonResult> GetAllocatedLeadData()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUser = user?.Id; // Use null-conditional operator for safety
            if (currentUser == null)
            {
                return Json(new { data = new List<object>() });
            }

            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true &&
                               lead.Assignees.Any(assignee => assignee.EmployeeId == currentUser) &&
                               lead.FollowUps.Any(followUp => followUp.Status == "Converted Lead") &&
                               _context.StudentMessages
                                   .Any(sm => sm.LeadId == lead.LeadId && sm.Status == "Approval"))
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
                CreatedDate = l.CreatedDate?.Date,
                ReferralName = l.Referral != null ? l.Referral.Name : null,
                EmployeeFirstName = l.Assignees.FirstOrDefault()?.Employee?.FirstName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status,
                AccountTitle = _context.Deposits
                                      .Where(p => p.UserId == currentUser && p.LeadId == l.LeadId)
                                      .Select(p => p.AccountTitle)
                                      .FirstOrDefault(),
                UniversityName = _context.Students
                                 .Where(s => s.LeadId == l.LeadId)
                                 .Include(s => s.University)
                                 .Select(s => s.University.Name)
                                 .FirstOrDefault(),
                CourseName = _context.Students
                             .Where(s => s.LeadId == l.LeadId)
                             .Include(s => s.Course)
                             .Select(s => s.Course.Name)
                             .FirstOrDefault() // Course Name
            });

            return Json(new { data = leadData });
        }

        [HttpPost]
        public async Task<IActionResult> PaymentDeposit(StudentProgramViewModel studentViewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var currentUser = user.Id;

                Deposit deposit = studentViewModel.Deposits;
                deposit.UserId = currentUser;

                IFormFile receiptFile = studentViewModel.DepositPayImagePath;

                if (receiptFile != null && receiptFile.Length > 0)
                {
                    // Ensure the deposit directory exists
                    string depositDir = Path.Combine(_webHostEnvironment.WebRootPath, "deposit");
                    if (!Directory.Exists(depositDir))
                    {
                        Directory.CreateDirectory(depositDir);
                    }

                    // Save the uploaded image
                    string fileName = $"{Guid.NewGuid()}_{receiptFile.FileName}";
                    string filePath = Path.Combine(depositDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await receiptFile.CopyToAsync(fileStream);
                    }

                    // Update the DepositPayImagePath property
                    deposit.DepositPayImagePath = $"/deposit/{fileName}";
                }

                // Check if a deposit with the given LeadId exists
                var existingDeposit = await _context.Deposits
                    .FirstOrDefaultAsync(d => d.LeadId == deposit.LeadId);

                if (existingDeposit != null)
                {
                    // Update the existing deposit
                    existingDeposit.AccountNumber = deposit.AccountNumber;
                    existingDeposit.Amount = deposit.Amount;
                    existingDeposit.DepositDate = deposit.DepositDate;
                    existingDeposit.DepositPayImagePath = deposit.DepositPayImagePath;
                    existingDeposit.AccountTitle = deposit.AccountTitle;
                    existingDeposit.Currency = deposit.Currency;
                    // Update other properties as needed
                }
                else
                {
                    // Add a new deposit
                    _context.Deposits.Add(deposit);
                }

                // Add activity log
                var activityLog = new ActivityLog
                {
                    LeadId = deposit.LeadId,
                    UserId = currentUser,
                    Status = "Make Deposit",
                    Action = "Payment Deposit Done",
                    ActivityLogDate = DateTime.Now
                };

                _context.ActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Deposit saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VisaApplication(StudentProgramViewModel studentViewModel)
        {
            try
            {
				var user = await _userManager.GetUserAsync(User);
				var currentUser = user.Id;
			 
                VisaApplication deposit = studentViewModel.VisaApplications;
                deposit.UserId = currentUser;

                // Check if a VisaApplication with the given LeadId already exists
                var existingApplication = await _context.VisaApplications
                    .FirstOrDefaultAsync(va => va.LeadId == deposit.LeadId);

                if (existingApplication != null)
                {
                    // Update existing record
                    existingApplication.PassportNumber = deposit.PassportNumber;
                    existingApplication.DestinationCountry = deposit.DestinationCountry;
                    existingApplication.VisaStatus = deposit.VisaStatus;
                    existingApplication.ExpiryDate = deposit.ExpiryDate;
                    existingApplication.UserId = deposit.UserId;

                    _context.VisaApplications.Update(existingApplication);
					var activityLog = new ActivityLog
					{
						LeadId = deposit.LeadId,
						UserId = currentUser,
						Status = "Visa Application Update",
						Action = "Visa Application Update",
						ActivityLogDate = DateTime.Now
					}; _context.ActivityLogs.Add(activityLog);
				}
                else
                {
                    // Add new record
                    _context.VisaApplications.Add(deposit);
					var activityLog = new ActivityLog
					{
						LeadId = deposit.LeadId,
						UserId = currentUser,
						Status = "Visa Application",
						Action = "Visa Application Submitted",
						ActivityLogDate = DateTime.Now
					};

					_context.ActivityLogs.Add(activityLog);
				}

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Visa application saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVisaApplicationByLeadId(int leadId)
        {
            try
            {
                var visaApplication = await _context.VisaApplications
                    .Where(va => va.LeadId == leadId)
                    .Select(va => new
                    {
                        va.PassportNumber,
                        va.VisaStatus,
                        va.ExpiryDate,
                        va.DestinationCountry
                    })
                    .FirstOrDefaultAsync();

                if (visaApplication != null)
                {
                    return Json(new { success = true, data = visaApplication });
                }
                else
                {
                    return Json(new { success = false, message = "No visa application found for this lead." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<JsonResult> GetVisaApplication()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUser = user?.Id; // Use null-conditional operator for safety

            if (currentUser == null)
            {
                return Json(new { data = new List<object>() });
            }

            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true &&
                               lead.Assignees.Any(assignee => assignee.EmployeeId == currentUser) &&
                               lead.FollowUps.Any(followUp => followUp.Status == "Converted Lead") &&
                               _context.StudentMessages
                                   .Any(sm => sm.LeadId == lead.LeadId && sm.Status == "Approval"))
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
                CreatedDate = l.CreatedDate?.Date,

                EmployeeFirstName = l.Assignees.FirstOrDefault()?.Employee?.FirstName,
                FollowUpStatus = l.FollowUps.FirstOrDefault()?.Status,
                VisaStatus = _context.VisaApplications
                             .Where(p => p.UserId == currentUser && p.LeadId == l.LeadId)
                             .Select(p => p.VisaStatus)
                             .FirstOrDefault(),
                UniversityName = _context.Students
                                 .Where(s => s.LeadId == l.LeadId)
                                 .Include(s => s.University)
                                 .Select(s => s.University.Name)
                                 .FirstOrDefault(),
                CourseName = _context.Students
                             .Where(s => s.LeadId == l.LeadId)
                             .Include(s => s.Course)
                             .Select(s => s.Course.Name)
                             .FirstOrDefault() // Course Name

            });

            return Json(new { data = leadData });
        }

        public async Task<JsonResult> EnrolledStudents()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUser = user?.Id;

            if (currentUser == null)
            {
                return Json(new { data = new List<object>() });
            }

            var allocatedLeads = _context.Leads
                .Where(lead => lead.IsLeadAssign == true &&
                               lead.Assignees.Any(assignee => assignee.EmployeeId == currentUser) &&
                               lead.FollowUps.Any(followUp => followUp.Status == "Converted Lead") &&
                               lead.Deposits.Any(d => d.Amount != null) &&
                               lead.VisaApplications.Any(v => v.VisaStatus == "accept"))
                .Include(l => l.Assignees).ThenInclude(lae => lae.Employee)
                .Include(l => l.Students)
                    .ThenInclude(s => s.University) // Include University
                .Include(l => l.Students)
                    .ThenInclude(s => s.Course) // Include Course
                .ToList();

            var leadData = allocatedLeads
                .SelectMany(lead => lead.Students, (lead, student) => new
                {
                    StudentFullName = $"{student.FirstName} {student.LastName}",
                    StudentEmail = student.Email,
                    StudentPhoneNumber = student.PhoneNumber,
                    Cnic = student.Cnic,
                    Gender = student.Gender,
                    UniversityName = student.University?.Name, // Ensure University is included
                    CourseName = student.Course?.Name // Ensure Course is included
                })
                .ToList();

            return Json(new { data = leadData });
        }
		public async Task<IActionResult> Detail(int id)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var currentUser = user?.Id;

				if (currentUser == null)
				{
					return Redirect("/Identity/Account/Login");
				}

				// Retrieve the student and related data
				var student = await _context.Students
					.Include(s => s.Course) // Include the Course navigation property
					.Include(s => s.University) // Include the University navigation property
					.FirstOrDefaultAsync(s => s.LeadId == id);

				if (student == null)
				{
					TempData["Error"] = "Student record not found.";
					return RedirectToAction("Form"); // Redirect to an appropriate action if student is not found
				}

				var leadfee = await _context.Consultationfees.FirstOrDefaultAsync(fee => fee.LeadId == student.LeadId);
				if (leadfee != null)
				{
					ViewBag.Amount = leadfee.Amount;
					ViewBag.Date = leadfee.Date;
				}

				ViewBag.LeadId = student.LeadId;

				var uniname = await _context.Universities.FindAsync(student.UniversityId);
				ViewBag.UniName = uniname?.Name;

				var cousrename = await _context.Courses.FindAsync(student.CourseId);
				ViewBag.Cousrename = cousrename?.Name;
				ViewBag.DurationInYears = cousrename?.DurationInYears;

				var unicour = await _context.UniversityCourses
					.FirstOrDefaultAsync(x => x.UniversityId == student.UniversityId && x.CourseId == student.CourseId);
				ViewBag.TuitionFee = unicour?.TuitionFee;

				// Retrieve deposits, visa applications, and educations associated with the student
				var deposits = await _context.Deposits
					.Where(d => d.LeadId == student.LeadId)
					.ToListAsync();

				var visaApplications = await _context.VisaApplications
					.Where(v => v.LeadId == student.LeadId)
					.ToListAsync();

				var educations = await _context.Educations
					.Where(e => e.LeadId == student.LeadId)
					.ToListAsync();

				// Create the view model
				var viewModel = new StudentProgramViewModel
				{
					Students = student,
					EducationsList = educations,
					DepositList = deposits,
					VisaApplicationList = visaApplications
				};

				return View(viewModel);
			}
			catch (Exception ex)
			{ 
				TempData["Error"] = "An error occurred while processing your request. Please try again later.";

			 
				return RedirectToAction("Error"); 
			}
		}

		public async Task<IActionResult> EditForm(int id)
        {
            var universities = await _context.Universities.ToListAsync();
             // Pass universities to the view
            ViewBag.Universities = universities;


            // Retrieve the student and related data
            var student = await _context.Students
                .Include(s => s.Course) // Include the Course navigation property
                .Include(s => s.University) // Include the University navigation property
                .FirstOrDefaultAsync(s => s.LeadId == id);
            ViewBag.Courses = await _context.Courses.ToListAsync();

            if (student == null)
            {
                TempData["Error"] = "Student record not found.";
                return RedirectToAction("Form"); // Redirect to an appropriate action if student is not found
            }

            string courseName = student.Course?.Name ?? "N/A";
            string universityName = student.University?.Name ?? "N/A";

            // Store course and university names in ViewBag
            ViewBag.CourseName = courseName;
            ViewBag.UniversityName = universityName;
            ViewBag.StudyGap = student.StudyGap;
       

           

            // Retrieve consultation fee
            var leadfee = await _context.Consultationfees
                .FirstOrDefaultAsync(fee => fee.LeadId == student.LeadId);

            // Retrieve university name
            var uniname = await _context.Universities
                .FindAsync(student.UniversityId);

            // Retrieve course name and duration
            var cousrename = await _context.Courses
                .FindAsync(student.CourseId);

            // Retrieve tuition fee
            var unicour = await _context.UniversityCourses
                .FirstOrDefaultAsync(x => x.UniversityId == student.UniversityId && x.CourseId == student.CourseId);

            // Set ViewBag properties for display
            ViewBag.UniName = uniname?.Name ?? "N/A";
            ViewBag.Cousrename = cousrename?.Name ?? "N/A";
            ViewBag.DurationInYears = cousrename?.DurationInYears;
            ViewBag.TuitionFee = unicour?.TuitionFee ?? 0;
            ViewBag.Amount = leadfee?.Amount ?? 0;
            ViewBag.Date = leadfee?.Date.HasValue == true ? leadfee.Date.Value.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd");
            ViewBag.Leadid = id;
            // Create the view model
            var viewModel = new StudentProgramViewModel
            {
                Students = student,
                EducationsList = await _context.Educations
                    .Where(e => e.LeadId == student.LeadId)
                    .ToListAsync(),
                DepositList = await _context.Deposits
                    .Where(d => d.LeadId == student.LeadId)
                    .ToListAsync(),
                VisaApplicationList = await _context.VisaApplications
                    .Where(v => v.LeadId == student.LeadId)
                    .ToListAsync()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStudentDetails(StudentProgramViewModel model)
        {
             

            // Retrieve the student record from the database
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.University)
                .FirstOrDefaultAsync(s => s.LeadId == model.Students.LeadId);

            if (student == null)
            {
                TempData["Error"] = "Student record not found.";
                return RedirectToAction("Form"); // Redirect to an appropriate action if student is not found
            }

            // Update student details
            student.FirstName = model.Students.FirstName;
            student.LastName = model.Students.LastName;
            student.PhoneNumber = model.Students.PhoneNumber;
            student.Email = model.Students.Email;
            student.Language = model.Students.Language;
            student.CourseId = model.Students.CourseId;
            student.UniversityId = model.Students.UniversityId;
             

            // Check and update related entities if needed (e.g., university, course, etc.)
            var university = await _context.Universities
                .FirstOrDefaultAsync(u => u.UniversityId == student.UniversityId);
            if (university != null)
            {
                // Update university related details if needed
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == student.CourseId);
            if (course != null)
            {
                // Update course related details if needed
            }

            var fee = await _context.Consultationfees
              .FirstOrDefaultAsync(c => c.LeadId == student.LeadId);
            if (fee != null)
            {
                fee.Amount = model.Fees.Amount;
                fee.Date = model.Fees.Date;
            }



            // Save changes to the database
            await _context.SaveChangesAsync();

            TempData["Success"] = "Student details updated successfully.";
            return RedirectToAction("EditForm", new { id = student.LeadId });
        }
        // Controller Action
        public async Task<IActionResult> GetCoursesByUniversity(int universityId)
        {
            var courses = await _context.UniversityCourses
                .Where(uc => uc.UniversityId == universityId)
                .Select(uc => new
                {
                    uc.Course.CourseId,
                    CourseName = uc.Course.Name,
                    uc.TuitionFee,
                    uc.Course.DurationInYears
                })
                .ToListAsync();

            return Json(courses);
        }
        [HttpPost]
        public IActionResult UpdateEducationField(int id, int leadId, string title2, string attachment2)
        {
            // Find the education record to update
            var education = _context.Educations
                .FirstOrDefault(e => e.EducationLevelId == id && e.LeadId == leadId);

            if (education != null)
            {
                // Update specific fields
                education.title2 = null; // or use an empty string "" if null is not allowed
                education.attachment2 = null; // or use an empty string "" if null is not allowed


                // Save changes
                _context.Educations.Update(education);
                _context.SaveChanges();
            }
            TempData["SuccessMessages"] = "Education delete successfully.";

            // Redirect to the appropriate action or view
            return RedirectToAction("EditForm", new { id = leadId });
        }
        [HttpPost]
        public IActionResult UpdateEducationField3(int id, int leadId, string title3, string attachment3)
        {
            // Find the education record to update
            var education = _context.Educations
                .FirstOrDefault(e => e.EducationLevelId == id && e.LeadId == leadId);

            if (education != null)
            {
                // Update specific fields
                education.title3 = null;
                education.attachment3 = null;

                // Save changes
                _context.Educations.Update(education);
                _context.SaveChanges();
            }
            TempData["SuccessMessages"] = "Education delete successfully.";

            // Redirect to the appropriate action or view
            return RedirectToAction("EditForm", new { id = leadId });
        }

        [HttpPost]
        public IActionResult UpdateEducationField4(int id, int leadId, string title4, string attachment4)
        {
            // Find the education record to update
            var education = _context.Educations
                .FirstOrDefault(e => e.EducationLevelId == id && e.LeadId == leadId);

            if (education != null)
            {
                // Update specific fields
                education.title4 = null;
                education.attachment4 = null;

                // Save changes
                _context.Educations.Update(education);
                _context.SaveChanges();
            }
            TempData["SuccessMessages"] = "Education delete successfully.";

            // Redirect to the appropriate action or view
            return RedirectToAction("EditForm", new { id = leadId });
        }

        [HttpPost]
        public IActionResult UpdateEducationField11(int id, int leadId, string title1, string EducationLevelImageUrl)
        {
            // Find the education record to update
            var education = _context.Educations
                .FirstOrDefault(e => e.EducationLevelId == id && e.LeadId == leadId);

            if (education != null)
            {
                // Update specific fields
                education.title1 = null;
                education.EducationLevelImageUrl = null;

                // Save changes
                _context.Educations.Update(education);
                _context.SaveChanges();
            }
            TempData["SuccessMessages"] = "Education delete successfully.";

            // Redirect to the appropriate action or view
            return RedirectToAction("EditForm", new { id = leadId });
        }



        [HttpPost]
        public async Task<IActionResult> UpdateEducation(StudentProgramViewModel vm)
        {
            // Ensure id is being passed or retrieved correctly
            var education = await _context.Educations
                .FirstOrDefaultAsync(e => e.LeadId == vm.Educations.LeadId);

            if (education == null)
            {
                return NotFound();
            }

            // Update education properties from ViewModel
            education.LeadId = vm.Educations.LeadId;
            if (vm.Educations.title1 != null)
            {
                education.title1 = vm.Educations.title1;
            }

            if (vm.Educations.title2 != null)
            {
                education.title2 = vm.Educations.title2;
            }

            if (vm.Educations.title3 != null)
            {
                education.title3 = vm.Educations.title3;
            }

            if (vm.Educations.title4 != null)
            {
                education.title4 = vm.Educations.title4;
            }

            // Handle file uploads
            if (vm.EducationImageFile != null && vm.EducationImageFile.Length > 0)
            {
                var educationFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Education");
                if (!Directory.Exists(educationFolder))
                {
                    Directory.CreateDirectory(educationFolder);
                }

                var educationFileName = Path.GetFileName(vm.EducationImageFile.FileName);
                var educationFilePath = Path.Combine(educationFolder, educationFileName);

                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(education.EducationLevelImageUrl))
                {
                    var oldFilePath = Path.Combine(educationFolder, Path.GetFileName(education.EducationLevelImageUrl));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(educationFilePath, FileMode.Create))
                {
                    await vm.EducationImageFile.CopyToAsync(fileStream);
                }

                education.EducationLevelImageUrl = Path.Combine("Education", educationFileName);
            }

            if (vm.DepositPayImagePath != null && vm.DepositPayImagePath.Length > 0)
            {
                var depositPayFolder = Path.Combine(_webHostEnvironment.WebRootPath, "DepositPay");
                if (!Directory.Exists(depositPayFolder))
                {
                    Directory.CreateDirectory(depositPayFolder);
                }

                var depositPayFileName = Path.GetFileName(vm.DepositPayImagePath.FileName);
                var depositPayFilePath = Path.Combine(depositPayFolder, depositPayFileName);

                // Handle file saving for DepositPayImagePath (if necessary)
                using (var fileStream = new FileStream(depositPayFilePath, FileMode.Create))
                {
                    await vm.DepositPayImagePath.CopyToAsync(fileStream);
                }

                // You might need to update the relevant property if applicable
            }

            if (vm.attachment2ImagePath != null && vm.attachment2ImagePath.Length > 0)
            {
                var attachmentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments");
                if (!Directory.Exists(attachmentFolder))
                {
                    Directory.CreateDirectory(attachmentFolder);
                }

                var attachment2FileName = Path.GetFileName(vm.attachment2ImagePath.FileName);
                var attachment2FilePath = Path.Combine(attachmentFolder, attachment2FileName);

                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(education.attachment2))
                {
                    var oldFilePath = Path.Combine(attachmentFolder, Path.GetFileName(education.attachment2));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(attachment2FilePath, FileMode.Create))
                {
                    await vm.attachment2ImagePath.CopyToAsync(fileStream);
                }

                education.attachment2 = Path.Combine("Attachments", attachment2FileName);
            }

            if (vm.attachment3ImagePath != null && vm.attachment3ImagePath.Length > 0)
            {
                var attachmentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments");
                if (!Directory.Exists(attachmentFolder))
                {
                    Directory.CreateDirectory(attachmentFolder);
                }

                var attachment3FileName = Path.GetFileName(vm.attachment3ImagePath.FileName);
                var attachment3FilePath = Path.Combine(attachmentFolder, attachment3FileName);

                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(education.attachment3))
                {
                    var oldFilePath = Path.Combine(attachmentFolder, Path.GetFileName(education.attachment3));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(attachment3FilePath, FileMode.Create))
                {
                    await vm.attachment3ImagePath.CopyToAsync(fileStream);
                }

                education.attachment3 = Path.Combine("Attachments", attachment3FileName);
            }

            if (vm.attachment4ImagePath != null && vm.attachment4ImagePath.Length > 0)
            {
                var attachmentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments");
                if (!Directory.Exists(attachmentFolder))
                {
                    Directory.CreateDirectory(attachmentFolder);
                }

                var attachment4FileName = Path.GetFileName(vm.attachment4ImagePath.FileName);
                var attachment4FilePath = Path.Combine(attachmentFolder, attachment4FileName);

                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(education.attachment4))
                {
                    var oldFilePath = Path.Combine(attachmentFolder, Path.GetFileName(education.attachment4));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(attachment4FilePath, FileMode.Create))
                {
                    await vm.attachment4ImagePath.CopyToAsync(fileStream);
                }

                education.attachment4 = Path.Combine("Attachments", attachment4FileName);
            }
            TempData["SuccessMessage"] = "Education Update successfully.";

            _context.Educations.Update(education);
            await _context.SaveChangesAsync();

            // Redirect to the appropriate action, ensure you pass the correct id
            return RedirectToAction("EditForm", new { id = vm.Educations.LeadId });
        }

    }

}

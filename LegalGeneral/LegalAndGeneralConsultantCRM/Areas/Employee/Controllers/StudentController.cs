using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using LegalAndGeneralConsultantCRM.ViewModels.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{

	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class StudentController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public StudentController(LegalAndGeneralConsultantCRMContext context, IWebHostEnvironment webHostEnvironment, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
		}
        // Get Domains based on selected university
        [HttpGet]
        public JsonResult GetDomainsByUniversity(int universityId)
        {
            var domains = _context.UniversityCourses
                .Where(uc => uc.UniversityId == universityId)
                .Select(uc => uc.Domain)
                .Where(d => d != null) // Ensure no null domains
                .GroupBy(d => new { d.DomainId, d.DomainNme })
                .Select(g => new SelectListItem
                {
                    Value = g.Key.DomainId.ToString(),
                    Text = g.Key.DomainNme
                })
                .ToList();

            return Json(domains);
        }


        // Get Courses based on selected domain
        [HttpGet]
        public JsonResult GetCoursesByDomain(int domainId)
        {
            var courses = _context.UniversityCourses
                .Where(uc => uc.DomainId == domainId)
                .Select(uc => uc.Course)
                .Distinct() // In case multiple entries have the same course
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.Name
                }).ToList();

            return Json(courses);
        }

        // Get Intakes based on selected course
        [HttpGet]
        public JsonResult GetIntakesByCourse(int courseId)
        {
            // Fetch the university course record based on courseId
            var universityCourse = _context.UniversityCourses
                .Where(uc => uc.CourseId == courseId)
                .Select(uc => new
                {
                    uc.Intake1,
                    uc.Intake2,
                    uc.Intake3
                })
                .FirstOrDefault();

            // Check if the universityCourse is null
            if (universityCourse == null)
            {
                // Return an empty list or handle as needed
                return Json(new List<string>());
            }

            // Collect the intakes into a list and filter out any null or empty values
            var intakes = new List<string>
    {
        universityCourse.Intake1,
        universityCourse.Intake2,
        universityCourse.Intake3
    }
            .Where(i => !string.IsNullOrEmpty(i))
            .ToList();

            return Json(intakes);
        }

        public async Task<IActionResult> StudentInfo(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead != null)
            {
                ViewBag.FirstName = lead.FirstName;
                ViewBag.LastName = lead.LastName;
                ViewBag.PhoneNumber = lead.PhoneNumber;
                ViewBag.Gender = lead.Gender;
                ViewBag.Email = lead.Email;
                var universities = await _context.Universities.ToListAsync();
                ViewBag.LeadId = lead.LeadId;
                // Pass universities to the view
                ViewBag.Universities = universities;
            }
            else
            {
                ViewBag.ErrorMessage = "Lead not found.";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCoursesByUniversity(int universityId)
        {
            var courses = await _context.UniversityCourses
                .Include(uc => uc.Course) // Include the Course navigation property
                .Where(uc => uc.UniversityId == universityId)
                .Select(uc => new
                {
                    CourseId = uc.CourseId,
                    CourseName = uc.Course.Name,  
                    DurationInYears = uc.Course.DurationInYears,
                    TuitionFee = uc.TuitionFee
                }).ToListAsync();

            return Json(courses);
        }

        [HttpPost]
        public IActionResult SaveEducation(Education education)
        {
            try
            {
                // Add LeadId from form data if it's not already set
                if (education.LeadId == null)
                {
                    // Ensure LeadId is set properly from the form submission
                    education.LeadId = ViewBag.LeadId;
                }

                // Add to database and save changes
                _context.Educations.Add(education);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception, you can use a logging framework like Serilog, NLog, etc.
                // Example: _logger.LogError(ex, "An error occurred while saving education.");

                // Handle the exception gracefully
                var errorMessage = "An error occurred while saving education. Please try again later.";
                return Json(new { success = false, errors = new[] { errorMessage } });
            }
        }



 
        public async Task<JsonResult> GetEducationByLead(int leadid)
        {
        try
                {
                    // Query database for educations related to the leadid, including AcademicRecords
                    var educations = await _context.Educations
                        .Include(e => e.AcademicRecords) // Eager load AcademicRecords
                        .Where(e => e.LeadId == leadid)
                        .Select(e => new
                        {
                            e.EducationLevelId,
                            e.StudentId,
                            e.LeadId,
                            LeadName = e.Lead.FirstName + " " + e.Lead.LastName, // Combine first and last name
                            EducationLevel =  e.EducationLevel,
                            e.EducationLevelImageUrl,
                            DegreeTitle=  e.DegreeTitle,
                            e.MajorSubject,
                            SchoolUni  = e.SchoolUni,
                            StartDate =   e.StartDate,
                            EndDate =  e.EndDate,
                            e.ObjMarks,
                            e.TotalMarks,
                            e.Division,
                            e.Percentage,
                            e.Board,
                            e.CGPA,
                             
                        })
                        .ToListAsync();

                    // Return JSON result
                    return Json(new { data = educations });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // Example: _logger.LogError(ex, "An error occurred while fetching education records.");

                    // Return error JSON
                    return Json(new { error = "An error occurred while fetching education records." });
                }
        }



        [HttpPost]
        public async Task<IActionResult> SaveStudent(StudentProgramViewModel studentViewModel,int UniversityId, string domain,int course,string intake)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserString = user.Id;

            try
            {
                Student student = studentViewModel.Students;
                Education education = studentViewModel.Educations;
                Consultationfee consultationfee = studentViewModel.Fees;
                IFormFile receiptFile = studentViewModel.ReceiptFile;
                IFormFile educationImageFile = studentViewModel.EducationImageFile;
                IFormFile depositPayImagePath = studentViewModel.DepositPayImagePath;
                IFormFile attachment2ImagePath = studentViewModel.attachment2ImagePath;
                IFormFile attachment3ImagePath = studentViewModel.attachment3ImagePath;
                IFormFile attachment4ImagePath = studentViewModel.attachment4ImagePath;

                Lead lead = await _context.Leads
                                          .Where(l => l.LeadId == student.LeadId)
                                          .FirstOrDefaultAsync();

                if (lead != null)
                {
                    lead.IsEnrolled = true;
                    var leadid = lead.LeadId;

                    _context.Leads.Update(lead);

                    var activityLog = new ActivityLog
                    {
                        LeadId = leadid,
                        UserId = currentUserString,
                        Status = "Student Details Submission",
                        Action = "Converted Lead Added to Student",
                        ActivityLogDate = DateTime.Now
                    };

                    _context.ActivityLogs.Add(activityLog);

                    // Handle file upload for receipt
                    if (receiptFile != null && receiptFile.Length > 0)
                    {
                        var receiptsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "receipt");
                        if (!Directory.Exists(receiptsFolder))
                        {
                            Directory.CreateDirectory(receiptsFolder);
                        }

                        var receiptFileName = Path.GetFileName(receiptFile.FileName);
                        var receiptFilePath = Path.Combine(receiptsFolder, receiptFileName);

                        using (var fileStream = new FileStream(receiptFilePath, FileMode.Create))
                        {
                            await receiptFile.CopyToAsync(fileStream);
                        }

                        consultationfee.ReceiptUrl = Path.Combine("receipt", receiptFileName);
                    }

                    // Handle file upload for education image
                    if (educationImageFile != null && educationImageFile.Length > 0)
                    {
                        var educationFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Education");
                        if (!Directory.Exists(educationFolder))
                        {
                            Directory.CreateDirectory(educationFolder);
                        }

                        var educationFileName = Path.GetFileName(educationImageFile.FileName);
                        var educationFilePath = Path.Combine(educationFolder, educationFileName);

                        using (var fileStream = new FileStream(educationFilePath, FileMode.Create))
                        {
                            await educationImageFile.CopyToAsync(fileStream);
                        }

                        education.EducationLevelImageUrl = Path.Combine("Education", educationFileName);
                    }

                    // Handle file upload for deposit pay image
                    if (depositPayImagePath != null && depositPayImagePath.Length > 0)
                    {
                        var depositPayFolder = Path.Combine(_webHostEnvironment.WebRootPath, "DepositPay");
                        if (!Directory.Exists(depositPayFolder))
                        {
                            Directory.CreateDirectory(depositPayFolder);
                        }

                        var depositPayFileName = Path.GetFileName(depositPayImagePath.FileName);
                        var depositPayFilePath = Path.Combine(depositPayFolder, depositPayFileName);

                        using (var fileStream = new FileStream(depositPayFilePath, FileMode.Create))
                        {
                            await depositPayImagePath.CopyToAsync(fileStream);
                        }

                        consultationfee.ReceiptUrl = Path.Combine("DepositPay", depositPayFileName);
                    }

                    // Handle file uploads for attachments
                    var attachmentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Attachments");
                    if (!Directory.Exists(attachmentFolder))
                    {
                        Directory.CreateDirectory(attachmentFolder);
                    }

                    if (attachment2ImagePath != null && attachment2ImagePath.Length > 0)
                    {
                        var attachment2FileName = Path.GetFileName(attachment2ImagePath.FileName);
                        var attachment2FilePath = Path.Combine(attachmentFolder, attachment2FileName);

                        using (var fileStream = new FileStream(attachment2FilePath, FileMode.Create))
                        {
                            await attachment2ImagePath.CopyToAsync(fileStream);
                        }

                        education.attachment2 = Path.Combine("Attachments", attachment2FileName);
                    }

                    if (attachment3ImagePath != null && attachment3ImagePath.Length > 0)
                    {
                        var attachment3FileName = Path.GetFileName(attachment3ImagePath.FileName);
                        var attachment3FilePath = Path.Combine(attachmentFolder, attachment3FileName);

                        using (var fileStream = new FileStream(attachment3FilePath, FileMode.Create))
                        {
                            await attachment3ImagePath.CopyToAsync(fileStream);
                        }

                        education.attachment3 = Path.Combine("Attachments", attachment3FileName);
                    }

                    if (attachment4ImagePath != null && attachment4ImagePath.Length > 0)
                    {
                        var attachment4FileName = Path.GetFileName(attachment4ImagePath.FileName);
                        var attachment4FilePath = Path.Combine(attachmentFolder, attachment4FileName);

                        using (var fileStream = new FileStream(attachment4FilePath, FileMode.Create))
                        {
                            await attachment4ImagePath.CopyToAsync(fileStream);
                        }

                        education.attachment4 = Path.Combine("Attachments", attachment4FileName);
                    }

                    student.Address = "";
                    student.Domicile = "";
                    student.IsDisable = false;
                    student.IsInternationalStudent = false;
                    student.UserId = currentUserString;
                    education.LeadId = student.LeadId;
                    consultationfee.LeadId = student.LeadId;
                    student.CourseId = course;
                    student.Intake = intake;
                    student.Domains = domain;
                    student.UniversityId = UniversityId;
                    student.StudyGap = student.StudyGap;
                    student.Applicantstatus = student.Applicantstatus;
                    // Save student information
                    await _context.Students.AddAsync(student);
                    await _context.Consultationfees.AddAsync(consultationfee);
                    await _context.Educations.AddAsync(education);

                    await _context.SaveChangesAsync();

                    TempData["Successu"] = "Student details saved successfully.";
                    return RedirectToAction("Form", "Application");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
            }

            return View();
        }




        // Old Code 

        public async Task<IActionResult> Index()
        {
            var followup = await _context.Students.ToListAsync();
            return View(followup);
        }
        public async Task<JsonResult> GetStudentDataAsync()
        {
            var students = await _context.Students.ToListAsync();

            return Json(new { data = students });
        }


        public async Task<IActionResult> AddStudent(int id)
        {
            ViewBag.AllStudents = _context.Students.ToList();
            ViewBag.AddUniversity = _context.Universities.ToList();

            ViewData["LeadId"] = id;


            return View();
        }


        //[HttpPost]
        //public async Task<IActionResult> AddStudent(StudentProgramViewModel studentViewModel)
        //{
        //    try
        //    {
        //        Student student = studentViewModel.Students;
        //        var leadId = studentViewModel.Students.LeadId;

        //        // Check if the student already exists based on LeadId
        //        Student existingStudent = await _context.Students
        //            .FirstOrDefaultAsync(s => s.LeadId == leadId);

        //        // Find the lead based on LeadId and IsEnrolled = false
        //        Lead lead = await _context.Leads
        //            .Where(l => l.LeadId == leadId && l.IsEnrolled == false)
        //            .FirstOrDefaultAsync();

        //        if (existingStudent != null)
        //        {
        //            existingStudent.LeadId = leadId;
        //            existingStudent.FirstName = student.FirstName;
        //            existingStudent.LastName = student.LastName;
        //            existingStudent.BirthDate = student.BirthDate;
        //            existingStudent.Email = student.Email;
        //            existingStudent.PhoneNumber = student.PhoneNumber;
        //            existingStudent.Address = student.Address;
        //            existingStudent.Gender = student.Gender;
        //            existingStudent.Cnic = student.Cnic;
        //            existingStudent.Domicile = student.Domicile;
        //            existingStudent.IsDisable = student.IsDisable;

        //            // Update additional details
        //            existingStudent.IsInternationalStudent = student.IsInternationalStudent;
        //            existingStudent.ProfileImageUrl = student.ProfileImageUrl;
        //            existingStudent.Nationality = student.Nationality;
        //            existingStudent.Major = student.Major;
        //            existingStudent.AdditionalInformation = student.AdditionalInformation;

        //            // Update emergency contact information
        //            existingStudent.EmergencyContactName = student.EmergencyContactName;
        //            existingStudent.EmergencyContactPhone = student.EmergencyContactPhone;

        //            // Update guardian information
        //            existingStudent.GuardianName = student.GuardianName;
        //            existingStudent.GuardianEmail = student.GuardianEmail;
        //            existingStudent.GuardianPhoneNumber = student.GuardianPhoneNumber;
        //            existingStudent.GuardianAddress = student.GuardianAddress;
        //            existingStudent.Relation = student.Relation;

        //            // Update health-related information
        //            existingStudent.BloodGroup = student.BloodGroup;

        //            // Update language information
        //            existingStudent.Language = student.Language;
        //            existingStudent.EmergencyAddress = student.EmergencyAddress;
        //            existingStudent.EmergencyPhoneNumber = student.EmergencyPhoneNumber;
        //            existingStudent.EmergencyName = student.EmergencyName;

                   

        //            _context.Students.Update(existingStudent);
        //            await _context.SaveChangesAsync();
                  

        //            if (lead != null)
        //            {
        //                // Update the IsEnrolled property
        //                lead.IsEnrolled = true;

        //                _context.Leads.Update(lead);

                    
        //                await _context.SaveChangesAsync();
 
        //                TempData["Success"] = "Personal Information created successfully";
        //                return RedirectToAction("AddStudent ", "Student", new { area = "Employee", id = leadId });
        //            }
        //        }

        //        else
        //        {
        //            if (lead != null)
        //            {
        //                // Update the IsEnrolled property
        //                lead.IsEnrolled = true;

        //                _context.Leads.Update(lead);
 
        //                _context.Students.Add(student);
        //                await _context.SaveChangesAsync();
        //            }
                
        //        }
        //        TempData["Success"] = "Personal Information created successfully";
              
        //        return RedirectToAction("AddStudent", "Student", new { area = "Employee", id = leadId });


        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception appropriately
        //        return View("Error");
        //    }
        //}


        public async Task<IActionResult> EducationDetails(int leadId)
        {
            var educationDetailsList = await _context.Educations
                .Where(e => e.LeadId == leadId)
                .ToListAsync();

            // Return data as JSON
            return Json(educationDetailsList);
        }


        [HttpPost]
        public async Task<IActionResult> Deposit(StudentProgramViewModel vm)
        {
            try
            {
                Deposit deposit = vm.Deposits;

                var leadId = vm.Students.LeadId;

                // Check if the student exists
                var student = await _context.Students.FirstOrDefaultAsync(s => s.LeadId == leadId);

                if (student == null)
                {
                    return NotFound();
                }

                // Check if a deposit already exists for the student
                var existingDeposit = await _context.Deposits.FirstOrDefaultAsync(d => d.StudentId == student.StudentId);

                if (existingDeposit == null)
                {
                    // If no existing deposit, create a new one
                    deposit.StudentId = student.StudentId;
                    deposit.LeadId = leadId;
                    _context.Deposits.Add(deposit);
                }
                else
                {
                    existingDeposit.LeadId = leadId;
                    existingDeposit.StudentId = student.StudentId; ;
                    existingDeposit.AccountNumber = deposit.AccountNumber;
                    existingDeposit.Amount = deposit.Amount;
                    existingDeposit.DepositDate = deposit.DepositDate;
                    existingDeposit.DepositPayImagePath = deposit.DepositPayImagePath;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Deposit Paid created successfully";

                return RedirectToAction("AddStudent", "Student", new { area = "Employee", id = leadId });
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> VisaApplication(StudentProgramViewModel vm)
        {
            try
            {
                VisaApplication visa = vm.VisaApplications;
                var leadId = vm.Students.LeadId;

                // Check if the student exists
                var student = await _context.Students.FirstOrDefaultAsync(s => s.LeadId == leadId);

                if (student == null)
                {
                    return NotFound();
                }

                // Check if a deposit already exists for the student
                var existingDeposit = await _context.VisaApplications.FirstOrDefaultAsync(d => d.StudentId == student.StudentId);

                if (existingDeposit == null)
                {
                    // If no existing deposit, create a new one
                    visa.StudentId = student.StudentId;
                    visa.LeadId = leadId;
                    _context.VisaApplications.Add(visa);
                }
                else
                {
                    existingDeposit.StudentId = student.StudentId;
                    existingDeposit.LeadId = leadId;
                    existingDeposit.ApplicationNumber = visa.ApplicationNumber;
                    existingDeposit.PassportNumber = visa.PassportNumber;
                    existingDeposit.DestinationCountry = visa.DestinationCountry;
                    existingDeposit.VisaStatus = visa.VisaStatus;
                    existingDeposit.SubmissionDate = visa.SubmissionDate;

                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "VisaApplication created successfully";

                return RedirectToAction("AddStudent", "Student", new { area = "Employee", id = leadId });
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return View("Error");
            }
        }


        public async Task<JsonResult> GetPrograms(int universityId)
        {
            try
            {
                // Retrieve the OfferProgram data for the selected university
                var programs = await _context.Universities
                    .Where(u => u.UniversityId == universityId)
                    .Select(u => u.OfferProgram)
                    .FirstOrDefaultAsync();

                // Check if programs were found
                if (programs != null)
                {
                    // Split the OfferProgram string into an array of programs
                    var programArray = programs.Split(',');

                    // Return the programs as JSON
                    return Json(programArray);
                }

                return Json(new string[] { }); // Return an empty array if no programs were found
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { error = "An error occurred while retrieving programs." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(StudentProgramViewModel model)
        {
            try
            {
                // Extract education information from the view model
                Education edu = model.Educations;

                // Find the corresponding student using LeadId
                int? leadId = edu.LeadId;
                if (leadId != null)
                {
                    // Find the student with the matching LeadId
                    Student student = await _context.Students
                        .FirstOrDefaultAsync(s => s.LeadId == leadId);

                    if (student != null)
                    {
                        // Associate the education with the found student
                        edu.StudentId = student.StudentId;

                        // Add the education to the context and save changes
                        _context.Educations.Add(edu);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Educational Information created successfully";

                        return RedirectToAction("AddStudent", "Student", new { area = "Employee", id = leadId });
                    }
                }

                // If LeadId is not valid or no matching student is found, return an error view
                return View("Error");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return View("Error");
            }
        }



        public async Task<IActionResult> StudentAcademia()
        {
            return View();
        }

        public async Task<JsonResult> StudentAcademicRecord()
        {
            var students = await _context.Students.ToListAsync();

            return Json(new { data = students });

        }


        public async Task<IActionResult> Detail(int id)
        {
            var student = await _context.Students
                .Include(s => s.Education)
                .Include(s => s.ProgramInTalks).ThenInclude(p => p.University)
                .Include(s => s.Deposits)  
                .Include(s => s.VisaApplications)  
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }




        [HttpPost]
        public async Task<IActionResult> StudentProgramInTalks(StudentProgramViewModel model)
        {
            int universityId = model.Universities.UniversityId;
 
            var university = await _context.Universities.FindAsync(universityId);

            Student edu = model.Students;
            // Find the corresponding student using LeadId
            int? leadId = edu.LeadId;
            if (leadId != null)
            {
                // Find the student with the matching LeadId
                Student stud = await _context.Students
                    .FirstOrDefaultAsync(s => s.LeadId == leadId);

                if (stud != null) // Corrected the variable name here
                {
                    // Associate the education with the found student
                    edu.StudentId = stud.StudentId;

                    string selectedProgramName = model.SelectedProgram;

                    ProgramInTalk programInTalks = new ProgramInTalk
                    {
                        StudentId = stud.StudentId, // Corrected the variable name here
                        UniversityId = universityId,
                        ProgramName = selectedProgramName,
                        LeadId = edu.LeadId // Assign stud.StudentId to LeadId
                    };

                    _context.ProgramInTalks.Add(programInTalks);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "ProgramInTalks created successfully";

                    return RedirectToAction("AddStudent", "Student", new { area = "Employee", id = leadId });
                }
            }
            return View();
        }




        public async Task<IActionResult> StudentStatus()
        {
             
            var studentsWithMessages = await _context.Students
                .Include(s => s.StudentMessages)  
                .ToListAsync();

            return View(studentsWithMessages);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);

            var educationsList = _context.Educations.Where(e => e.StudentId == id).ToList();
            var programInTalkList = _context.ProgramInTalks.Where(p => p.StudentId == id).ToList();

            var deposits = _context.Deposits.Where(d => d.StudentId == id).ToList();
            var visaApplications = _context.VisaApplications.Where(va => va.StudentId == id).ToList();

            var viewModel = new StudentProgramViewModel
            {
                Students = student,
                EducationsList = educationsList,
                ProgramInTalkList = programInTalkList,
                Deposits = deposits.FirstOrDefault(), // Adjust this based on your business logic
                VisaApplications = visaApplications.FirstOrDefault(), // Adjust this based on your business logic
                                                                      // Add other related data to the view model
            };

            ViewBag.AllStudents = _context.Students.ToList();
            ViewBag.AddUniversity = _context.Universities.ToList();
            int? leadId = student.LeadId;
            int studentId = student.StudentId;

            // Pass the LeadId as a ViewBag to the view
            ViewBag.LeadId = leadId;
            ViewBag.StudentId = studentId;


            return View(viewModel);
        }

        public async Task<IActionResult> NewEducation(int studentId)
        {
            // Assuming you have a DbSet<Student> in your DbContext
            var student = await _context.Students
                .Include(s => s.Education) // Include the Education navigation property
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                // Handle the case where the student with the provided ID is not found
                return NotFound();
            }

            // Assuming LeadId is a property of the Student entity
            var leadId = student.LeadId;

            // Store studentId and leadId in ViewBag
            ViewBag.StudentId = studentId;
            ViewBag.LeadId = leadId;

            return View();
        }

        [HttpPost]
        public ActionResult EditStudentProgram(StudentProgramViewModel viewModel)
        {
            try
            {
                var leadId = viewModel.Educations.LeadId;
                var studentId = viewModel.Educations.StudentId;

                // Retrieve existing ProgramInTalk entity
                var existingProgram = _context.ProgramInTalks
                    .SingleOrDefault(p => p.LeadId == leadId && p.StudentId == studentId);
                string selectedProgramName = viewModel.SelectedProgram;

                if (existingProgram != null)
                {
                    // Update the properties with the selected values from the form
                    existingProgram.UniversityId = viewModel.Universities.UniversityId;
                    existingProgram.StudentId = studentId;
                    existingProgram.LeadId = leadId;
                    existingProgram.ProgramName = selectedProgramName;

                    // No need to call Update explicitly when using EF Core
                    // _context.ProgramInTalks.Update(existingProgram);

                    _context.SaveChanges();

                    TempData["Success"] = "Student Program  have been successfully saved.";
                    
                    return RedirectToAction("Edit", "Student", new { id = studentId });

                }
                else
                {
                    // If the program is not found, you should create a new one instead of adding it directly
                    var newProgram = new ProgramInTalk
                    {
                        UniversityId = viewModel.Universities.UniversityId,
                        StudentId = studentId,
                        LeadId = leadId,
                        ProgramName = selectedProgramName
                    };

                    _context.ProgramInTalks.Add(newProgram);

                    _context.SaveChanges();

                    TempData["Success"] = "Deposit pay have been successfully saved.";
                    
                    return RedirectToAction("Edit", "Student", new { id = studentId });

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                ModelState.AddModelError("", "An error occurred while saving changes.");
            }

            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult EditDeposit(StudentProgramViewModel viewModel)
        {
            try
            {
                var leadId = viewModel.Educations.LeadId;
                var studentId = viewModel.Educations.StudentId;

                // Retrieve existing ProgramInTalk entity
                var existingProgram = _context.Deposits
                    .SingleOrDefault(p => p.LeadId == leadId && p.StudentId == studentId);
 
                if (existingProgram != null)
                {
                    // Update the properties with the selected values from the form
                    existingProgram.StudentId = studentId;
                    existingProgram.LeadId = leadId;
                    existingProgram.AccountNumber = viewModel.Deposits.AccountNumber;
                    existingProgram.Amount = viewModel.Deposits.Amount;
                    existingProgram.DepositDate = viewModel.Deposits.DepositDate;
                    existingProgram.DepositPayImagePath = viewModel.Deposits.DepositPayImagePath;

                    // No need to call Update explicitly when using EF Core
                    // _context.ProgramInTalks.Update(existingProgram);

                    _context.SaveChanges();
                    TempData["Success"] = "Deposit pay have been successfully saved.";
                    // Redirect to a success page or perform other actions
                    // Redirect to the "Edit" action of the "Student" controller with the student's ID
                    return RedirectToAction("Edit", "Student", new { id = studentId });
                }
                else
                {
                    // If the program is not found, you should create a new one instead of adding it directly
                    var newProgram = new Deposit
                    {
                        AccountNumber = viewModel.Deposits.AccountNumber,
                        Amount = viewModel.Deposits.Amount,
                        DepositDate = viewModel.Deposits.DepositDate,
                        DepositPayImagePath = viewModel.Deposits.DepositPayImagePath,
                        StudentId = studentId,
                        LeadId = leadId,
                        
                    };

                    _context.Deposits.Add(newProgram);

                    _context.SaveChanges();

                    TempData["Success"] = "Deposit pay have been successfully saved.";
                    // Redirect to the "Edit" action of the "Student" controller with the student's ID
                    return RedirectToAction("Edit", "Student", new { id = studentId });

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                ModelState.AddModelError("", "An error occurred while saving changes.");
            }

            return View("Edit", viewModel);
        }


        [HttpPost]
        public ActionResult EditVisa(StudentProgramViewModel viewModel)
        {
            try
            {
                var leadId = viewModel.Educations.LeadId;
                var studentId = viewModel.Educations.StudentId;

                // Retrieve existing ProgramInTalk entity
                var existingProgram = _context.VisaApplications
                    .SingleOrDefault(p => p.LeadId == leadId && p.StudentId == studentId);

                if (existingProgram != null)
                {
                    // Update the properties with the selected values from the form
                    existingProgram.StudentId = (int)studentId;
                    existingProgram.LeadId = leadId;
                    existingProgram.PassportNumber = viewModel.VisaApplications.PassportNumber;
                    existingProgram.ApplicationNumber = viewModel.VisaApplications.ApplicationNumber;
                    existingProgram.SubmissionDate = viewModel.VisaApplications.SubmissionDate;
                    existingProgram.DestinationCountry = viewModel.VisaApplications.DestinationCountry;
                    existingProgram.VisaStatus = viewModel.VisaApplications.VisaStatus;
                   

                    // No need to call Update explicitly when using EF Core
                    // _context.ProgramInTalks.Update(existingProgram);

                    _context.SaveChanges();
                    TempData["Success"] = "Visa Application have been successfully saved.";
                    // Redirect to the "Edit" action of the "Student" controller with the student's ID
                    return RedirectToAction("Edit", "Student", new { id = studentId });
                }
                else
                {
                    // If the program is not found, you should create a new one instead of adding it directly
                    var newProgram = new VisaApplication
                    {
                        PassportNumber = viewModel.VisaApplications.PassportNumber,
                        ApplicationNumber = viewModel.VisaApplications.ApplicationNumber,
                        SubmissionDate = viewModel.VisaApplications.SubmissionDate,
                        DestinationCountry = viewModel.VisaApplications.DestinationCountry,
                        VisaStatus = viewModel.VisaApplications.VisaStatus,
                        
                      
                        StudentId = (int)studentId,
                        LeadId = leadId,

                    };

                    _context.VisaApplications.Add(newProgram);

                    _context.SaveChanges();
                    TempData["Success"] = "Visa Application have been successfully saved.";
                    // Redirect to the "Edit" action of the "Student" controller with the student's ID
                    return RedirectToAction("Edit", "Student", new { id = studentId });

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                ModelState.AddModelError("", "An error occurred while saving changes.");
            }

            return View("Edit", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> NewEducation(StudentProgramViewModel viewModel)
        {
            // Retrieve LeadId and StudentId from the view model
            var leadId = viewModel.Educations.LeadId;
            var studentId = viewModel.Educations.StudentId;

            // Assuming you have a DbSet<Education> in your DbContext
            var education = new Education
            {
                LeadId = leadId,
                StudentId = studentId,

                // Assign other properties from the view model or form data
                EducationLevel = viewModel.Educations.EducationLevel,
                EducationLevelImageUrl = viewModel.Educations.EducationLevelImageUrl,
                DegreeTitle = viewModel.Educations.DegreeTitle,
                MajorSubject = viewModel.Educations.MajorSubject,
                SchoolUni = viewModel.Educations.SchoolUni,
                StartDate = viewModel.Educations.StartDate,
                EndDate = viewModel.Educations.EndDate,
                ObjMarks = viewModel.Educations.ObjMarks,
                TotalMarks = viewModel.Educations.TotalMarks,
                Division = viewModel.Educations.Division,
                Percentage = viewModel.Educations.Percentage,
                Board = viewModel.Educations.Board,
                CGPA = viewModel.Educations.CGPA,
            };

            // Add the education to the context
            await _context.Educations.AddAsync(education);

            // Save changes to the database
            await _context.SaveChangesAsync();


            TempData["Success"] = "Educational records have been successfully saved.";

             

            // Redirect to the "Edit" action of the "Student" controller with the student's ID
            return RedirectToAction("Edit", "Student", new { id = studentId });
        }




        [HttpPost]
        public async Task<IActionResult> EditEducation(StudentProgramViewModel viewModel)
        {
            // Retrieve LeadId and StudentId from the view model
            var leadId = viewModel.Educations.LeadId;
            var studentId = viewModel.Educations.StudentId;
            var educationLevelId = viewModel.Educations.EducationLevelId;

            var existingEducation = await _context.Educations
                .FirstOrDefaultAsync(e => e.EducationLevelId == educationLevelId);

            if (existingEducation == null)
            {
                // No existing record found, create a new one
                var newEducation = new Education
                {
                    LeadId = leadId,
                    StudentId = studentId,

                    // Map all fields from the view model
                    EducationLevel = viewModel.Educations.EducationLevel,
                    EducationLevelImageUrl = viewModel.Educations.EducationLevelImageUrl,
                    DegreeTitle = viewModel.Educations.DegreeTitle,
                    MajorSubject = viewModel.Educations.MajorSubject,
                    SchoolUni = viewModel.Educations.SchoolUni,
                    StartDate = viewModel.Educations.StartDate,
                    EndDate = viewModel.Educations.EndDate,
                    ObjMarks = viewModel.Educations.ObjMarks,
                    TotalMarks = viewModel.Educations.TotalMarks,
                    Division = viewModel.Educations.Division,
                    Percentage = viewModel.Educations.Percentage,
                    Board = viewModel.Educations.Board,
                    CGPA = viewModel.Educations.CGPA,
                     
                };

                await _context.Educations.AddAsync(newEducation);
            }
            else
            {
                // Existing record found, update its properties
                _context.Educations.Update(existingEducation);

                // Map all fields from the view model
                existingEducation.EducationLevel = viewModel.Educations.EducationLevel;
                existingEducation.EducationLevelImageUrl = viewModel.Educations.EducationLevelImageUrl;
                existingEducation.DegreeTitle = viewModel.Educations.DegreeTitle;
                existingEducation.MajorSubject = viewModel.Educations.MajorSubject;
                existingEducation.SchoolUni = viewModel.Educations.SchoolUni;
                existingEducation.StartDate = viewModel.Educations.StartDate;
                existingEducation.EndDate = viewModel.Educations.EndDate;
                existingEducation.ObjMarks = viewModel.Educations.ObjMarks;
                existingEducation.TotalMarks = viewModel.Educations.TotalMarks;
                existingEducation.Division = viewModel.Educations.Division;
                existingEducation.Percentage = viewModel.Educations.Percentage;
                existingEducation.Board = viewModel.Educations.Board;
                existingEducation.CGPA = viewModel.Educations.CGPA;
                existingEducation.LeadId = leadId;
                existingEducation.StudentId = studentId;

             }

            // Save changes to the database
            await _context.SaveChangesAsync();

            TempData["Success"] = "Educational records have been successfully saved.";

            // Redirect to the "Edit" action of the "Student" controller with the student's ID
            return RedirectToAction("Edit", "Student", new { id = studentId });
        }







        //[HttpPost]
        //public async Task<IActionResult> EditStudent(StudentProgramViewModel viewModel)
        //{
             
        //        try
        //        {
        //        // Retrieve the existing student entity from the database
        //        var existingStudent = await _context.Students
        //            .Include(s => s.Lead)  // Include the Lead navigation property if needed
        //            .FirstOrDefaultAsync(s => s.StudentId == viewModel.Students.StudentId);

        //        if (existingStudent != null)
        //        {
        //                int? leadId = existingStudent.Lead?.LeadId;
        //                existingStudent.LeadId = leadId;
        //                existingStudent.FirstName = viewModel.Students.FirstName;
        //                existingStudent.LastName = viewModel.Students.LastName;
        //                existingStudent.Email = viewModel.Students.Email;
        //                existingStudent.PhoneNumber = viewModel.Students.PhoneNumber;
        //                existingStudent.BirthDate = viewModel.Students.BirthDate;
        //                existingStudent.Gender = viewModel.Students.Gender;
        //                existingStudent.Cnic = viewModel.Students.Cnic;
        //                existingStudent.Domicile = viewModel.Students.Domicile;
        //                existingStudent.Address = viewModel.Students.Address;


        //                existingStudent.IsDisable = viewModel.Students.IsDisable;
        //                existingStudent.IsInternationalStudent = viewModel.Students.IsInternationalStudent;
        //                existingStudent.ProfileImageUrl = viewModel.Students.ProfileImageUrl;
        //                existingStudent.Nationality = viewModel.Students.Nationality;
        //                existingStudent.Major = viewModel.Students.Major;
        //                existingStudent.AdditionalInformation = viewModel.Students.AdditionalInformation;

        //                // Update Emergency Contact information
        //                existingStudent.EmergencyContactName = viewModel.Students.EmergencyContactName;
        //                existingStudent.EmergencyContactPhone = viewModel.Students.EmergencyContactPhone;

        //                // Update Guardian information
        //                existingStudent.GuardianName = viewModel.Students.GuardianName;
        //                existingStudent.GuardianEmail = viewModel.Students.GuardianEmail;
        //                existingStudent.GuardianPhoneNumber = viewModel.Students.GuardianPhoneNumber;
        //                existingStudent.GuardianAddress = viewModel.Students.GuardianAddress;
        //                existingStudent.Relation = viewModel.Students.Relation;

        //                // Update Health-related information
        //                existingStudent.BloodGroup = viewModel.Students.BloodGroup;

        //                // Update Language information
        //                existingStudent.Language = viewModel.Students.Language;
        //                existingStudent.EmergencyAddress = viewModel.Students.EmergencyAddress;
        //                existingStudent.EmergencyPhoneNumber = viewModel.Students.EmergencyPhoneNumber;
        //                existingStudent.EmergencyName = viewModel.Students.EmergencyName;

                        
        //                _context.Students.Update(existingStudent);
        //                // Save changes to the database
        //                await _context.SaveChangesAsync();
        //            TempData["Success"] = "Student records have been successfully saved.";
        //            return RedirectToAction("Index", "Student"); // Redirect to the student list or details page






        //                _context.Students.Update(existingStudent);
        //                // Save changes to the database
        //                await _context.SaveChangesAsync();

        //                return RedirectToAction("Index", "Student"); // Redirect to the student list or details page
        //            }
        //            else
        //            {
        //                // Handle the case where the student with the specified ID is not found
        //                return NotFound();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the exception or handle it appropriately
        //            return View("Error");
        //        }
          
        //    // If ModelState is not valid, return to the edit view with validation errors
        //    return View(viewModel);
        //}



        public async Task<IActionResult> EditEducation(int id)
        {
            // Assuming you have a DbContext named 'dbContext'
            var education = await _context.Educations.FindAsync(id);

            if (education == null)
            {
                // Handle the case where the education record with the given id is not found
                return NotFound();
            }

            // Store LeadId and StudentId in ViewBag
            ViewBag.LeadId = education.LeadId;
            ViewBag.StudentId = education.StudentId;
            ViewBag.EducationLevelId = education.EducationLevelId;

            // Create an instance of StudentProgramViewModel and populate it with the necessary data
            var viewModel = new StudentProgramViewModel
            {
                // Populate other properties of the view model as needed
                Educations = education
            };

            // Your additional logic for the EditEducation action goes here...

            return View(viewModel);
        }



        



    }

}

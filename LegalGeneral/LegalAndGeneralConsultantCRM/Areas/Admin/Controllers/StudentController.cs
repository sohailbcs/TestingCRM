using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using LegalAndGeneralConsultantCRM.ViewModels.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]

    public class StudentController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
       
        public StudentController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
            
        }


        
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


        public async Task<IActionResult> AddStudent()
        {
            ViewBag.AllStudents = _context.Students.ToList();
            ViewBag.AddUniversity = _context.Universities.ToList();
 
            

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddStudent(StudentProgramViewModel studentViewModel)
        {
            try
            {
                 Student student = studentViewModel.Students;

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
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
                int studentId = vm.Students.StudentId;

                // Check if the student exists
                var student = await _context.Students.FindAsync(studentId);

                if (student == null)
                {
                    return NotFound();
                }

                // Check if a deposit already exists for the student
                var existingDeposit = await _context.VisaApplications.FirstOrDefaultAsync(d => d.StudentId == studentId);

                if (existingDeposit == null)
                {
                    // If no existing deposit, create a new one
                    visa.StudentId = studentId;
                    _context.VisaApplications.Add(visa);
                }
                else
                {
                    existingDeposit.ApplicationNumber = visa.ApplicationNumber;
                    existingDeposit.PassportNumber = visa.PassportNumber;
                    existingDeposit.DestinationCountry = visa.DestinationCountry;
                    existingDeposit.VisaStatus = visa.VisaStatus;
                    existingDeposit.SubmissionDate = visa.SubmissionDate;

                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
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

                        return RedirectToAction("Index");
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

        public async Task<JsonResult> StudentData()
        {
            var studentsWithMessages = await _context.Students
                .Include(s => s.University) // Include University entity
                .Include(s => s.Course) // Include Course entity
                .Include(s => s.Lead) // Include Lead entity
                    .ThenInclude(l => l.FollowUps) // Include FollowUps in Lead entity
                        .ThenInclude(f => f.Employee) // Include Employee in FollowUps
                .Select(s => new
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    FullName = s.FirstName + " " + s.LastName, // Concatenate first name and last name
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    UniversityName = s.University.Name, // Include University name
                    CourseName = s.Course.Name, // Include Course name
                    LeadId = s.Lead.LeadId,
                    EmployeeFullName = s.Lead.FollowUps
                        .Where(f => f.LeadId == s.LeadId)
                        .Select(f => f.Employee.FirstName + " " + f.Employee.LastName)
                        .FirstOrDefault(),
                    Messages = string.Join("<br/>", _context.StudentMessages
                .Where(sm => sm.LeadId == s.Lead.LeadId)
                .Select(sm => sm.Message)
                .ToArray()),
                    Statuses = string.Join("<br/>", _context.StudentMessages
                .Where(sm => sm.LeadId == s.Lead.LeadId)
                .Select(sm => sm.Status)
                .ToArray())
                })
                .ToListAsync();

            return Json(new { data = studentsWithMessages });
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Retrieve the student and related data
            var student = await _context.Students
                .Include(s => s.Course) // Include the Course navigation property
                .Include(s => s.University) // Include the University navigation property
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            // Retrieve lead fee data
            var leadfee = await _context.Consultationfees.FirstOrDefaultAsync(fee => fee.LeadId == student.LeadId);
            ViewBag.Amount = leadfee?.Amount ?? 0; // Use 0 if leadfee or Amount is null
            ViewBag.Date = leadfee?.Date ?? DateTime.MinValue; // Use DateTime.MinValue as default date

            // Retrieve student lead information
            var studeny = await _context.Students.FirstOrDefaultAsync(fee => fee.LeadId == student.LeadId);
            ViewBag.LeadId = studeny?.LeadId ?? 0; // Use 0 if studeny or LeadId is null

            // Retrieve university name
            var uniname = await _context.Universities.FindAsync(student.UniversityId);
            ViewBag.UniName = uniname?.Name ?? "Unknown"; // Use "Unknown" if uniname or Name is null

            // Retrieve course name and duration
            var cousrename = await _context.Courses.FindAsync(student.CourseId);
            ViewBag.Cousrename = cousrename?.Name ?? "Unknown"; // Use "Unknown" if cousrename or Name is null
            ViewBag.DurationInYears = cousrename?.DurationInYears?? "notset"; // Use 0 if DurationInYears is null

            // Retrieve university-course information
            var unicour = await _context.UniversityCourses
                .FirstOrDefaultAsync(x => x.UniversityId == student.UniversityId && x.CourseId == student.CourseId);
            ViewBag.TuitionFee = unicour?.TuitionFee ?? 0; // Use 0 if unicour or TuitionFee is null
            var unicour2 = await _context.UniversityCourses
              .FirstOrDefaultAsync(x => x.UniversityId == student.UniversityId && x.CourseId == student.CourseId);
            ViewBag.TuitionFees = unicour?.Currency ?? "not set"; // Use 0 if unicour or TuitionFee is null

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
        [HttpPost]
		public async Task<IActionResult> ApplicationStatus(StudentProgramViewModel model)
		{
            try
            {
                var student = await _context.Students
                                   .Where(s => s.StudentId == model.Students.StudentId)
                                   .Select(s => new { s.UserId })
                                   .FirstOrDefaultAsync();
                var studentName = await _context.Students
                                   .Where(s => s.StudentId == model.Students.StudentId)
                                   .Select(s => new { s.FirstName })
                                   .FirstOrDefaultAsync();

                var userid = model.Students.UserId;
				// Retrieve an existing StudentMessage based on LeadId
				var existingMessage = await _context.StudentMessages
					.FirstOrDefaultAsync(sm => sm.LeadId == model.StudentMessages.LeadId);

				// Create a new StudentMessage instance
				StudentMessage studentMessage = new StudentMessage
				{
					LeadId = model.StudentMessages.LeadId,
					Message = model.StudentMessages.Message,
					Status = model.StudentMessages.Status,
					CreatedAt = DateTime.Now
				};

				if (existingMessage == null)
                {
                    var notifications = new Notification
                    {
                        UserId = student.UserId, // EmployeeId is saved as UserId
                        Message = "the status of"+ studentName+" please check in application table for latest update",
                        NotificationTime = DateTime.Now,
                        IsRead = false
                    };
                    // No existing record found, add a new one
                    _context.StudentMessages.Add(studentMessage);
				}
				else
				{
					// Existing record found, update it
					existingMessage.Message = studentMessage.Message;
					existingMessage.Status = studentMessage.Status;
					existingMessage.CreatedAt = studentMessage.CreatedAt;

					_context.StudentMessages.Update(existingMessage);
				}

				// Save changes to the database
				await _context.SaveChangesAsync();

				// Use TempData to store the success message
				TempData["SuccessMessage"] = "Student message saved successfully.";
				return RedirectToAction("StudentAcademia", "Student", new { area = "Admin" });
			}
			catch (Exception ex)
			{
				// Log the exception (optional)
				// _logger.LogError(ex, "An error occurred while saving the student message.");

				// Use TempData to store the error message
				TempData["ErrorMessage"] = "An error occurred while saving the student message.";

				// Return to the form with the model and error message
				return View(model);
			}
		}



		[HttpPost]
        public async Task<IActionResult> StudentProgramInTalks(StudentProgramViewModel model)
        {
            int studentId = model.Students.StudentId;
            int universityId = model.Universities.UniversityId;

             var student = await _context.Students.FindAsync(studentId);
            var university = await _context.Universities.FindAsync(universityId);

             if (student == null || university == null)
            {
                 return NotFound();
            }

             string selectedProgramName = model.SelectedProgram;

             ProgramInTalk programInTalks = new ProgramInTalk
            {
                StudentId = studentId,
                UniversityId = universityId,
                ProgramName = selectedProgramName,
              };

             _context.ProgramInTalks.Add(programInTalks);
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentMessage(StudentProgramViewModel vm)
        {
            try
            {
                
                    StudentMessage studt = vm.StudentMessages;

                    // Check if it's a new message or an update
                    if (studt.StudentId > 0)
                    {
                        // It's an update, so retrieve the existing message from the database
                        var existingMessage = await _context.StudentMessages.FindAsync(studt.StudentId);

                        if (existingMessage != null)
                        {
                            // Update the existing message properties
                            existingMessage.LeadId = studt.LeadId;
                            existingMessage.Message = studt.Message;
                            existingMessage.CreatedAt = DateTime.Now; // You may want to adjust the timestamp logic
                            existingMessage.Status =studt.Status;
                            _context.StudentMessages.Update(existingMessage);

                            // Update the database
                            await _context.SaveChangesAsync();

                        return RedirectToAction("StudentAcademia", "Student", new { area = "Admin" });
                    }
                    else
                    {
                        // It's a new message, create and save it
                        StudentMessage studentMessage = new StudentMessage
                        {
                            StudentId = studt.StudentId,
                            LeadId = studt.LeadId,
                            Message = studt.Message,
                            CreatedAt = DateTime.Now,
                            Status = studt.Status
                        };

                        _context.StudentMessages.Add(studentMessage);
                        await _context.SaveChangesAsync();
                    }
                }


                return RedirectToAction("StudentAcademia", "Student", new { area = "Admin" });



            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Enrollment(StudentProgramViewModel vm)
        {
            var leadId = vm.StudentEnrollments.LeadId;
            var studentId = vm.StudentEnrollments.StudentId;

            if (leadId != null && studentId != null)
            {
                // Check if enrollment already exists
                var existingEnrollment = await _context.StudentEnrollments
                    .FirstOrDefaultAsync(e => e.LeadId == leadId && e.StudentId == studentId);

                if (existingEnrollment != null)
                {
                    existingEnrollment.StudentId = studentId;
                    existingEnrollment.LeadId = leadId;
                    existingEnrollment.EnrollmentStatus = vm.StudentEnrollments.EnrollmentStatus;
                    existingEnrollment.EnrollmentDate = vm.StudentEnrollments.EnrollmentDate;
                    existingEnrollment.IsAccepted = true;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("StudentAcademia", "Student", new { area = "Admin" });
                }
                else
                {
                    // Enrollment doesn't exist, create a new one
                    var enrollment = new StudentEnrollment
                    {
                        LeadId = leadId,
                        StudentId = studentId,
                        EnrollmentStatus = vm.StudentEnrollments.EnrollmentStatus,
                        EnrollmentDate = vm.StudentEnrollments.EnrollmentDate,
                         IsAccepted = true
                    };

                    _context.StudentEnrollments.Add(enrollment);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("StudentAcademia", "Student", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



    }

}

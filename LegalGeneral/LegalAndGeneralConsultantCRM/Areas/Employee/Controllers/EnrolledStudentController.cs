using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class EnrolledStudentController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public EnrolledStudentController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
        {

            return View();
        }
        public async Task<JsonResult> GetEnrolledStudentData()
        {
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			// Get current user ID from session
		 

            // Fetch enrolled students along with their associated Lead information
            var enrolledStudents = await _context.StudentEnrollments
                .Include(se => se.Lead)
                .Where(se => se.EnrollmentStatus == "Enrolled" && se.UserId == currentUser)
                .Select(se => new
                {
                    Student = _context.Students
                            .Where(d => d.LeadId == se.Lead.LeadId)
                            .Select(d => new
                            {
                                FirstName =  d.FirstName,
                                LastName =   d.LastName,
                                FullName = d.FirstName + " " + d.LastName, // Concatenated FullName

                                d.PhoneNumber,
                                d.Cnic,
                                d.Email,
                                d.CourseId,
                                d.UniversityId,
                                UniversityName = _context.Universities
                                    .Where(u => u.UniversityId == d.UniversityId)
                                    .Select(u => u.Name)
                                    .FirstOrDefault(),
                                CourseName = _context.Courses
                                    .Where(c => c.CourseId == d.CourseId)
                                    .Select(c => c.Name)
                                    .FirstOrDefault(),
                            })
                            .FirstOrDefault(),

                    Deposits = _context.Deposits
                            .Where(d => d.LeadId == se.Lead.LeadId)
                            .Select(d => new
                            {
                                d.DepositId,
                                d.Amount,
                                d.DepositDate,
                                d.AccountTitle,
                                d.AccountNumber,
                                d.DepositPayImagePath
                            })
                            .ToList(),

                    VisaApplications = _context.VisaApplications
                            .Where(va => va.LeadId == se.Lead.LeadId)
                            .Select(va => new
                            {
                                va.ApplicationId,
                                va.PassportNumber,
                                va.SubmissionDate,
                                va.DestinationCountry,
                                va.ApplicationNumber,
                                va.VisaStatus
                            })
                            .ToList()
                })
                .ToListAsync();

            return Json(new { data = enrolledStudents });
        }



        ////public async Task<JsonResult> GetEnrolledStudentData()
        ////{
        ////    var enrolledStudents = await _context.StudentEnrollments
        ////        .Include(se => se.Student)
        ////        .Include(se => se.Lead)
        ////        .Where(se => se.EnrollmentStatus == "Approved")
        ////        .ToListAsync();

        ////    var responseData = enrolledStudents.Select(se => new
        ////    {
        ////        StudentId = se.Student.StudentId,
        ////        FirstName = se.Student.FirstName,
        ////        LastName = se.Student.LastName,
        ////        PhoneNumber = se.Student.PhoneNumber,
        ////        Address = se.Student.Address,

        ////        UniName = _context.ProgramInTalks
        ////            .Where(p => p.StudentId == se.Student.StudentId)
        ////            .Select(p => p.University.Name)  
        ////            .FirstOrDefault(),


        ////        ProgramName = _context.ProgramInTalks
        ////            .Where(p => p.StudentId == se.Student.StudentId)
        ////            .Select(p => p.ProgramName)
        ////            .FirstOrDefault(),


        ////        AccountNumber = _context.Deposits
        ////            .Where(p => p.StudentId == se.Student.StudentId)
        ////            .Select(p => p.AccountNumber)
        ////            .FirstOrDefault(),

        ////        ApplicationNumber = _context.VisaApplications
        ////            .Where(p => p.StudentId == se.Student.StudentId)
        ////            .Select(p => p.ApplicationNumber)
        ////            .FirstOrDefault(),
        ////    });

        ////    return Json(new { data = responseData });
        ////}



    }

}

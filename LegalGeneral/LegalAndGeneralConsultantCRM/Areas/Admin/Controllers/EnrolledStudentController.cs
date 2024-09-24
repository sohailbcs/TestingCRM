using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class EnrolledStudentController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
       
        public EnrolledStudentController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
            
        }


        
        public async Task<IActionResult> Index()
        {
            
            return View();
        }


        public async Task<JsonResult> GetEnrolledStudentData()
        {
            var enrolledStudents = await _context.StudentEnrollments
                .Include(se => se.Student)
                .Include(se => se.Lead)
                .Where(se => se.EnrollmentStatus == "Approved")
                .ToListAsync();

            var responseData = enrolledStudents.Select(se => new
            {
                StudentId = se.Student.StudentId,
                FirstName = se.Student.FirstName,
                LastName = se.Student.LastName,
                PhoneNumber = se.Student.PhoneNumber,
                Address = se.Student.Address,
                FullName = se.Student.FirstName + " " + se.Student.LastName,

                UniName = _context.ProgramInTalks
                    .Where(p => p.StudentId == se.Student.StudentId)
                    .Select(p => p.University.Name)  
                    .FirstOrDefault(),


                ProgramName = _context.ProgramInTalks
                    .Where(p => p.StudentId == se.Student.StudentId)
                    .Select(p => p.ProgramName)
                    .FirstOrDefault(),


                AccountNumber = _context.Deposits
                    .Where(p => p.StudentId == se.Student.StudentId)
                    .Select(p => p.AccountNumber)
                    .FirstOrDefault(),

                ApplicationNumber = _context.VisaApplications
                    .Where(p => p.StudentId == se.Student.StudentId)
                    .Select(p => p.ApplicationNumber)
                    .FirstOrDefault(),
            });

            return Json(new { data = responseData });
        }




    }

}

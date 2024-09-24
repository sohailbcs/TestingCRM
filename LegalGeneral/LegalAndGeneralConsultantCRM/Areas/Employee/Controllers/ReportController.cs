using AspNetCore.Reporting;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
   
    public class ReportController : Controller
	{
		private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebHostEnvironment _Env;
		private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

		public ReportController(LegalAndGeneralConsultantCRMContext context, IWebHostEnvironment webHostEnvironment, IWebHostEnvironment env, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_Env = env;
			_userManager = userManager;
		}

		public async Task<IActionResult> UniverisityCourses()
        {
            var universities = await _context.Universities.ToListAsync();

            var universitySelectList = universities.Select(u => new
            {
                Id = u.UniversityId,
                Name = u.Name
            }).ToList();

            ViewBag.UniversityList = new SelectList(universitySelectList, "Id", "Name");

            return View();
        }

        public JsonResult GetUniCourses(int uniId)
        {

            var courses = _context.UniversityCourses
                .Include(uc => uc.Course)
                .Where(uc => uc.UniversityId == uniId)
                .Select(uc => new
                {
                    ProgramName = uc.Course.Name,
                    Duration = uc.Course.DurationInYears,
                    Fees = uc.TuitionFee
                })
                .ToList();

            return Json(new { data = courses });
        }
        public async Task<IActionResult> EnrolledStudent()
        {

            return View();
        }
		public async Task<JsonResult> GetEnrolledStudent()
		{
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;

			var enrolledStudents = await (from visa in _context.VisaApplications
										  join student in _context.Students on visa.LeadId equals student.LeadId
										  join course in _context.Courses on student.CourseId equals course.CourseId
										  join university in _context.Universities on student.UniversityId equals university.UniversityId
										  join lead in _context.Leads on student.LeadId equals lead.LeadId
										  where visa.VisaStatus == "accept" && lead.UserId == currentUser
										  select new
										  {
											  FullName = student.FirstName + " " + student.LastName,
											  PhoneNumber = student.PhoneNumber,
											  Email = student.Email,
											  CourseName = course.Name,
											  UniversityName = university.Name
										  }).ToListAsync();  // Use ToListAsync for async operation

			return Json(new { data = enrolledStudents });
		}



		public async Task<IActionResult> PrintEnrolledStudentsReport()
        {
			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			var result = (from visa in _context.VisaApplications
                          join student in _context.Students on visa.LeadId equals student.LeadId
                          join course in _context.Courses on student.CourseId equals course.CourseId
                          join university in _context.Universities on student.UniversityId equals university.UniversityId
                          join lead in _context.Leads on student.LeadId equals lead.LeadId
                          where visa.VisaStatus == "accept" && lead.UserId == currentUser
                          select new
                          {
                              FullName = student.FirstName + " " + student.LastName,
                              PhoneNumber = student.PhoneNumber,
                              Email = student.Email,
                              CourseName = course.Name,
                              UniversityName = university.Name
                          }).ToList();

            DataTable dataTable = Helpers.HelpersMethod.ListToDataTable(result);

            // Generate the report using RDLC
            var report = GenerateStudentReports(dataTable, "EnrolledReport", "pdf"); // Ensure 'pdf' is passed for PDF generation
            return report;
        }
        public IActionResult GenerateStudentReports(DataTable tab, string reportname, string export = "pdf")
        {
            int extension = 1; // Assuming this is for PDF extension, adjust if needed for Excel

            // Path to your RDLC file
            var path = $"{this._Env.WebRootPath}\\Reports\\{reportname}.rdlc";

            // Add parameters if needed
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            LocalReport localReport = new LocalReport(path);

            // Add the data source
            localReport.AddDataSource("Enrolled", tab);

            // Determine render type and content type based on 'export' parameter
            var render = export == "excel" ? RenderType.Excel : RenderType.Pdf;
            var contentType = export == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";

            // Execute the report rendering
            var result = localReport.Execute(render, extension);

            // Return the generated file
            return File(result.MainStream, contentType);
        }

        public async Task<IActionResult> LeadsGenration()
        {



            return View();
        }


    
   

    }
}

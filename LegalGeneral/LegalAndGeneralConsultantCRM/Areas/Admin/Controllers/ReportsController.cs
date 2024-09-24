using AspNetCore.Reporting;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    
    public class ReportsController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly IWebHostEnvironment _Env;

        public ReportsController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _Env = env;
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
           
            var courses =  _context.UniversityCourses
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

        public async Task<IActionResult> PrintUniversityReport(int uniId)
        {
            var uni = _context.Universities.Where(x => x.UniversityId == uniId).FirstOrDefault();
            var name = uni.Name;

            var result = _context.UniversityCourses
               .Include(uc => uc.Course)
               .Where(uc => uc.UniversityId == uniId)
               .Select(uc => new
               {
                   ProgramName = uc.Course.Name,
                   Duration = uc.Course.DurationInYears,
                   Fees = uc.TuitionFee,
                   UniName = name,

               })
               .ToList();

            DataTable dataTable = Helpers.HelpersMethod.ListToDataTable(result);
            var report = UniReports(dataTable, "UniversityReport", "");
 

            return report;
        }



        public async Task<IActionResult> AssignLeadsStatus()
        {
            var users = await _userManager.GetUsersInRoleAsync("Employee");

            // Assuming that your user model has properties FirstName and LastName
            var userSelectList = users.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}" // Combine FirstName and LastName
            }).ToList();

            ViewBag.UserList = new SelectList(userSelectList, "Id", "FullName");


            return View();
        }

        public JsonResult GetStatus(string teamMemberId)
        {
            var leadData = _context.LeadAssignEmployees
                .Include(lae => lae.Lead)
                    .ThenInclude(lead => lead.FollowUps) // Include FollowUps for Lead
                .Include(lae => lae.Employee)
                .Where(lae => lae.EmployeeId == teamMemberId)
                .ToList();

            var result = leadData.Select(ld => new
            {
                FirstName = ld.Lead.FirstName,
                FirstName1 = ld.Employee.FirstName,
                LastName1 = ld.Employee.LastName,
                LastName = ld.Lead.LastName,
                PhoneNumber = ld.Lead.PhoneNumber,
                Email = ld.Lead.Email,
                Status = ld.Lead.FollowUps.Select(f => f.Status).FirstOrDefault(),
                FullName = ld.Lead.FirstName + " " + ld.Lead.LastName,
            });

            return Json(new { data = result });
        }

        public async Task<IActionResult> Print(string teamMemberId)
        {
            // Fetch the user based on teamMemberId
            var user = await _context.Users.FindAsync(teamMemberId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch lead data associated with the user
            var leadData = _context.LeadAssignEmployees
                .Include(lae => lae.Lead)
                    .ThenInclude(lead => lead.FollowUps) // Include FollowUps for Lead
                .Include(lae => lae.Employee)
                .Where(lae => lae.EmployeeId == teamMemberId)
                .ToList();

            var result = leadData.Select(ld => new
            {
                FirstName = ld.Lead.FirstName,
                LastName = ld.Lead.LastName,
                   
                
                PhoneNumber = ld.Lead.PhoneNumber,
                Email = ld.Lead.Email,
                Status = ld.Lead.FollowUps.Select(f => f.Status).FirstOrDefault(),
                FullName = ld.Lead.FirstName + " " + ld.Lead.LastName,
                Team = ld.Employee.FirstName + " " + ld.Employee.LastName,
            }).ToList();

            DataTable dataTable = Helpers.HelpersMethod.ListToDataTable(result);
            var report = GenerateReports(dataTable, "LeadsStatusReport", "");

            // Include the user full name in the view bag
            ViewBag.UserFullName = user.FirstName + " " + user.LastName;

            return report;
        }





        public async Task<IActionResult> LeadsGenration()
        {
             


            return View();
        }


        public JsonResult GetLeadData(string fromDate, string toDate)
        {
            DateOnly fromDateValue, toDateValue;

            if (DateOnly.TryParse(fromDate, out fromDateValue) && DateOnly.TryParse(toDate, out toDateValue))
            {
                DateTime fromDateWithTime = new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day);
                DateTime toDateWithTime = new DateTime(toDateValue.Year, toDateValue.Month, toDateValue.Day);


                var leadData =
                                _context.Leads
                                .Where(lead => lead.CreatedDate >= fromDateWithTime && lead.CreatedDate <= toDateWithTime).Include(l => l.Referral)
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
                                .ToList();


                return Json(new { data = leadData });
            }
            else
            {
                // Handle invalid date formats or other errors
                return Json(new { error = "Invalid date format" });
            }
        }


        public async Task<IActionResult> EnrolledStudent()
        {

            return View();
        }
        public JsonResult GetEnrolledStudent()
        {
            var enrolledStudents = (from visa in _context.VisaApplications
                                    join student in _context.Students on visa.LeadId equals student.LeadId
                                    join course in _context.Courses on student.CourseId equals course.CourseId
                                    join university in _context.Universities on student.UniversityId equals university.UniversityId
                                    where visa.VisaStatus == "accept"
                                    select new
                                    {
                                        FullName = student.FirstName + " " + student.LastName,
                                        PhoneNumber = student.PhoneNumber,
                                        Email = student.Email,
                                        CourseName = course.Name,
                                        UniversityName = university.Name
                                    }).ToList();

            return Json(new { data = enrolledStudents });
        }



        public async Task<IActionResult> PrintEnrolledStudentsReport()
        {
            var result = (from visa in _context.VisaApplications
                                    join student in _context.Students on visa.LeadId equals student.LeadId
                                    join course in _context.Courses on student.CourseId equals course.CourseId
                                    join university in _context.Universities on student.UniversityId equals university.UniversityId
                                    where visa.VisaStatus == "accept"
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


        public async Task<IActionResult> LeadsPrint(string fromDate, string toDate)
        {
            DateOnly fromDateValue, toDateValue;

            if (DateOnly.TryParse(fromDate, out fromDateValue) && DateOnly.TryParse(toDate, out toDateValue))
            {
                DateTime fromDateWithTime = new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day);
                DateTime toDateWithTime = new DateTime(toDateValue.Year, toDateValue.Month, toDateValue.Day);


                var result =
                                _context.Leads
                                .Where(lead => lead.CreatedDate >= fromDateWithTime && lead.CreatedDate <= toDateWithTime)
                                .Select(lead => new
                                {
                                    Id = lead.LeadId,
                                    FirstName = lead.FirstName,
                                    LastName = lead.LastName,
                                    FullName = lead.FirstName + " " + lead.LastName, // Concatenate FirstName and LastName
                                    Email = lead.Email,
                                    PhoneNumber = lead.PhoneNumber,
                                    Gender = lead.Gender,
                                    LeadSource = lead.LeadSource,
                                    CreatedDate = lead.CreatedDate
                                })
                                .ToList();


                DataTable dataTable = Helpers.HelpersMethod.ListToDataTable(result);
                return GenerateLeadReports(dataTable, "LeadRPT", "");

            }

            return View();
        }
      
 

        public IActionResult GenerateLeadReports(DataTable tab, string reportname, string export = "pdf")
        {

            //string mintype = "";
            int extension = 1;
            var path = $"{this._Env.WebRootPath}\\Reports\\{reportname}.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            // parameters.Add("LabName", "Welcome To Life Care Diagnostic Lab ");
            //parameters.Add("CompanyName", reportTitle);
            LocalReport localReport = new LocalReport(path);

            localReport.AddDataSource("dsLeads", tab);

             
            var render = export == "excel" ? RenderType.Excel : RenderType.Pdf;
            var ContentType = export == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            //string name = "Calcium Report" + (export == "excel" ? ".xls" : ".pdf");
            var result = localReport.Execute(render, extension);
            return File(result.MainStream, ContentType);
           

        }


        public IActionResult GenerateReports(DataTable tab,  string reportname, string export = "pdf")
        {

            //string mintype = "";
            int extension = 1;
            var path = $"{this._Env.WebRootPath}\\Reports\\{reportname}.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            // parameters.Add("LabName", "Welcome To Life Care Diagnostic Lab ");
            //parameters.Add("CompanyName", reportTitle);
            LocalReport localReport = new LocalReport(path);

            localReport.AddDataSource("dsLeadsStatus", tab);

            //Desplay Image
            //string imageparam = "";
            //var imagepath = $"{this._Env.WebRootPath}\\img\\Blogo.jpeg";
            //using (var b = new Bitmap(imagepath))
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        b.Save(ms, ImageFormat.Bmp);
            //        imageparam = Convert.ToBase64String(ms.ToArray());
            //    }
            //}
            //parameters.Add("logoImage", imageparam);

            var render = export == "excel" ? RenderType.Excel : RenderType.Pdf;
            var ContentType = export == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            //string name = "Calcium Report" + (export == "excel" ? ".xls" : ".pdf");
            var result = localReport.Execute(render, extension);
            return File(result.MainStream, ContentType);
            //return File(result.MainStream, ContentType, name);

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

        public IActionResult UniReports(DataTable tab, string reportname, string export = "pdf")
        {

            //string mintype = "";
            int extension = 1;
            var path = $"{this._Env.WebRootPath}\\Reports\\{reportname}.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            // parameters.Add("LabName", "Welcome To Life Care Diagnostic Lab ");
            //parameters.Add("CompanyName", reportTitle);
            LocalReport localReport = new LocalReport(path);

            localReport.AddDataSource("dsUniversity", tab);

            //Desplay Image
            //string imageparam = "";
            //var imagepath = $"{this._Env.WebRootPath}\\img\\Blogo.jpeg";
            //using (var b = new Bitmap(imagepath))
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        b.Save(ms, ImageFormat.Bmp);
            //        imageparam = Convert.ToBase64String(ms.ToArray());
            //    }
            //}
            //parameters.Add("logoImage", imageparam);

            var render = export == "excel" ? RenderType.Excel : RenderType.Pdf;
            var ContentType = export == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            //string name = "Calcium Report" + (export == "excel" ? ".xls" : ".pdf");
            var result = localReport.Execute(render, extension);
            return File(result.MainStream, ContentType);
            //return File(result.MainStream, ContentType, name);

        }
    }
}
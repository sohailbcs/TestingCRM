using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class ClubMemberController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
       
        public ClubMemberController(LegalAndGeneralConsultantCRMContext context)
        {
            _context = context;
          
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

       
      

        public JsonResult GetService()
        {
            var brands = _context.ClubMembers.ToList();

            return Json(new { data = brands });
        }
    }

}

using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Employee.Controllers
{
	[Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class LeadConversionController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        public LeadConversionController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var referrals = await _context.Referrals.ToListAsync();
            //var brands = await _context.Brands.ToListAsync();
            //var product = await _context.Products.ToListAsync();

            ViewData["LeadId"] = id;

            ViewBag.Referral = new SelectList(referrals, "ReferralId", "Name");
            //ViewBag.Brands = new SelectList(brands, "BrandId", "ProjectName");
            //ViewBag.Project = new SelectList(product, "ProductId", "ProductName");

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(LeadsConversionDetail model)
        //{
        //    try
        //    {
        //        var currentUserId = await _userManager.GetUserAsync(User);
        //        model.UserId = currentUserId.Id;

        //         var existingRecord = await _context.LeadsConversionDetails
        //            .FirstOrDefaultAsync(x => x.UserId == model.UserId);

        //        if (existingRecord != null)
        //        {
        //            existingRecord.LeadId = model.LeadId;
        //            existingRecord.SoldProject = model.SoldProject;
        //            existingRecord.SoldProduct = model.SoldProduct;
        //            existingRecord.ProductId = model.ProductId;
        //            existingRecord.BrandId = model.BrandId;
        //            existingRecord.SoldProduct = model.SoldProduct;
        //            existingRecord.SalePrice = model.SalePrice;
        //            existingRecord.DownPayment = model.DownPayment;
        //            existingRecord.BalancePayment = model.BalancePayment;
        //            existingRecord.ReferralId = model.ReferralId;
        //            existingRecord.CommissionPaid = model.CommissionPaid;
        //            existingRecord.LeadConversionDate = DateOnly.FromDateTime(DateTime.Today);

        //            _context.LeadsConversionDetails.Update(existingRecord);
        //        }
        //        else
        //        {
        //            model.LeadConversionDate = DateOnly.FromDateTime(DateTime.Today);

        //            _context.LeadsConversionDetails.Add(model);
        //        }

        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Lead conversion details saved successfully.";


        //        return RedirectToAction("CustomersConversion");
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error");
        //    }



        public async Task<IActionResult> CustomersConversion()
        {

            return View();
        }


        //public async Task<JsonResult> GetConversion()
        //{
        //    var leadsConversionDetails = _context.LeadsConversionDetails
        //        .Include(lcd => lcd.Referral)
        //        .Include(lcd => lcd.Product)
        //        .Include(lcd => lcd.Brand)
        //        .Include(lcd => lcd.CRMUser)
        //        .Include(lcd => lcd.Lead)
        //        .ToList();

        //    var data = leadsConversionDetails.Select(lcd => new
        //    {
        //        lcd.LeadConversionDetailId,
        //        lcd.SoldProject,
        //        lcd.SoldProduct,
        //        lcd.SalePrice,
        //        lcd.DownPayment,
        //        lcd.BalancePayment,
        //        lcd.CommissionPaid,
        //        lcd.LeadConversionDate,
        //        BrandName = lcd.Brand?.ProjectName,
        //        ProductName = lcd.Product?.ProductName,
        //        ReferralName = lcd.Referral?.Name,
        //        User = _context.Users.FirstOrDefault(u => u.Id == lcd.UserId)?.FirstName,
        //        LeadFirstName = lcd.Lead?.FirstName,  
        //    });

        //    return Json(new { data });
        //}


    }
          
}

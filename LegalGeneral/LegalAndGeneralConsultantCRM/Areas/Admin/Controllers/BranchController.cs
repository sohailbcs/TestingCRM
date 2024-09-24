using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.Branches;
using LegalAndGeneralConsultantCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class BranchController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

        public BranchController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Franchises()
        {
            return View();
        }
        public async Task<IActionResult> GetFranchises()
        {
            var franchises = await _context.Frenchise.ToListAsync();
            return Json(new { data = franchises });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Frenchise franchise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(franchise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(franchise);
        }
        // Index action to render the view with branches data
        public async Task<IActionResult> Index()
        {
            var branches = await _context.Branches
                .Select(b => new BranchViewModel
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName ?? "empty",
                    BranchType = b.branchType ?? "empty",
                    BranchCode = b.BranchCode ?? "empty",
                    City = b.City ?? "empty",
                    Description = b.Description ?? "empty"
                })
                .ToListAsync();

            return View(branches);
        }


        public async Task<JsonResult> GetBranches()
        {
            var branches = await _context.Branches
                .Select(b => new
                {
                    b.BranchId,
                    BranchName = b.BranchName ?? "empty",
                    BranchType = b.branchType ?? "empty",
                    BranchCode = b.BranchCode ?? "empty",
                    City = b.City ?? "empty",
                    Description = b.Description ?? "empty"
                })
                .ToListAsync();

            return Json(new { data = branches });
        }

        [HttpPost]
        public async Task<IActionResult> AddBranch([FromBody] Branch branch)
        {
            if (ModelState.IsValid)
            {
                // Check if the BranchCode already exists in the database
                bool branchCodeExists = await _context.Branches
                    .AnyAsync(b => b.BranchCode == branch.BranchCode);

                if (branchCodeExists)
                {
                    return Json(new { success = false, message = "Branch Code already exists." });
                }

                // If the BranchCode does not exist, add the new branch
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Branch added successfully." });
            }

            return Json(new { success = false, message = "Failed to add branch." });
        }

      [HttpPost]
public async Task<IActionResult> EditBranch([FromBody] Branch branch)
{
    if (ModelState.IsValid)
    {
        var existingBranch = await _context.Branches.FindAsync(branch.BranchId);
        if (existingBranch != null)
        {
            existingBranch.BranchName = branch.BranchName;
            existingBranch.BranchCode = branch.BranchCode;
            existingBranch.City = branch.City;
            existingBranch.branchType = branch.branchType; // Update branchType
            existingBranch.Description = branch.Description;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Branch updated successfully." });
        }
        return Json(new { success = false, message = "Branch not found." });
    }
    return Json(new { success = false, message = "Failed to update branch." });
}

        // POST: Admin/Branch/DeleteBranch
     
        [HttpPost]
        public async Task<IActionResult> DeleteBranch([FromBody] int branchId)
        {
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Branch deleted successfully." });
            }
            return Json(new { success = false, message = "Branch not found." });
        }
    }
}


using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class EmployeeController : Controller
    {

        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;
        private readonly SignInManager<LegalAndGeneralConsultantCRMUser> _signInManager;  
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager, SignInManager<LegalAndGeneralConsultantCRMUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> User()
        {

            return View();
        }
        [HttpGet]
        public JsonResult GetBranchesByType(string branchType)
        {
            var branches = _context.Branches
                .Where(b => b.branchType == branchType)
                .Select(b => new
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName
                })
                .ToList();

            return Json(branches);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string id, bool status)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.Status = status; // Assuming `Status` is a property of the user model
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = status ? "User activated successfully." : "User deactivated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error while updating user status." });
            }
        }

        public JsonResult GetUserData()
        {
            var employeeRoleId = _roleManager.FindByNameAsync("Employee").Result?.Id;

            if (string.IsNullOrEmpty(employeeRoleId))
            {
                // Handle the case where the role "Employee" is not found
                return Json(new { data = new List<object>() });
            }

            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var branches = _context.Branches.ToList();
             var userData = employees.Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName,
               Fullname= user.FirstName + " "+ user.LastName ,
               user.Applicationprocessor,
                user.Email,
                user.PhoneNumber,
                user.Gender,
                user.Status,
                BranchName = user.Branch != null ? user.Branch.BranchName : "No Branch"

            });

            return Json(new { data = userData });
        }
        public IActionResult EmployeeRegister()
        {
            ViewBag.BranchTypes = new SelectList(new List<string> { "Branch", "Franchise" });
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> EmployeeRegister(EmployeeRegisterViewModel model, [FromServices] RoleManager<IdentityRole> roleManager)
        {
            // Check if the email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
				TempData["Error"] = "Email An account with this email already exists.";
                return RedirectToAction("User");

            }

            // Check if the phone number already exists
            var existingUserByPhoneNumber = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUserByPhoneNumber != null)
            {
				TempData["Error"] = "PhoneNumber An account with this phone number already exists";
                return RedirectToAction("User");
            }

            var user = new LegalAndGeneralConsultantCRMUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                CompanyName = model.CompanyName,
                JobTitle = model.JobTitle,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country, 
                Notes = model.Notes,
                Department = model.Department,
                TeamMemeberType = model.TeamMemeberType,
                BrandId = model.BrandId,
                Applicationprocessor =model.Applicationprocessor,
                Status =true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Check if the "Employee" role exists, and create it if it doesn't
                if (!await roleManager.RoleExistsAsync("Employee"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Employee"));
                }

                // Assign the "Employee" role to the user
                await _userManager.AddToRoleAsync(user, "Employee");

                TempData["Success"] = "Team Member is registered successfully.";

                return RedirectToAction("User", new { area = "Admin", controller = "Employee" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay the form
            return View(model);
        }


        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }

            var model = new EmployeeRegisterViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                CompanyName = user.CompanyName,
                JobTitle = user.JobTitle,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country,
                Notes = user.Notes,
                Department = user.Department,
                TeamMemeberType = user.TeamMemeberType,
                BrandId = user.BrandId,
                BranchType = user.BranchType,
                Applicationprocessor = (bool)user.Applicationprocessor,
                 
            };

            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName", user.BrandId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeRegisterViewModel model)
        {
            

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.CompanyName = model.CompanyName;
            user.JobTitle = model.JobTitle;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.ZipCode = model.ZipCode;
            user.Country = model.Country;
            user.Notes = model.Notes;
            user.Department = model.Department;
            user.TeamMemeberType = model.TeamMemeberType;
            user.BrandId = model.BrandId;
            user.Applicationprocessor = model.Applicationprocessor;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Team Member details updated successfully.";
                return RedirectToAction("User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchName", model.BrandId);
            return View(model);
        }

        public IActionResult ChangePassword(string id)
        {


            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.UserId = id;

            // Return the view with the userId in the query string
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> ChangePassword(EmployeeRegisterViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ChangePassword");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, vm.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password changed successfully.";
                return Redirect("/Admin/Employee/User");
            }

            foreach (var error in result.Errors)
            {
                TempData["ErrorMessage"] = error.Description;
            }

            return RedirectToAction("ChangePassword");
        }


    }
}


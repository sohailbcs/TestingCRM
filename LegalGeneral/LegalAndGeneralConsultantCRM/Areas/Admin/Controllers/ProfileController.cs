using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProfileController : Controller
    {
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

        public ProfileController(UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Pass the user details to the view using ViewBag
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;

            return View(user); // Pass the user model to the view
        }

        [HttpPost]
        public async Task<IActionResult> Index(LegalAndGeneralConsultantCRMUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Update user details
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            // Update the user in the database
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Show a success message
                ViewBag.Message = "Profile updated successfully.";
                return View(user); // Return the updated user model
            }

            // If update fails, display error messages
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user); // Return the user model with errors
        }


        public IActionResult ChangePassword()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(EmployeeRegisterViewModel model)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if the user is not found
            }

            // Check if the old password is correct
            var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.ExistingPassword);
            if (!isOldPasswordCorrect)
            {
                TempData["ErrorMessage"] = "The old password is incorrect.";
                return View(model); // Return the view with the error message
            }

            // Change the password
            var result = await _userManager.ChangePasswordAsync(user, model.ExistingPassword, model.ConfirmPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been changed successfully!";
                // Password changed successfully, redirect to a confirmation page or profile page
                return LocalRedirect("/Admin/Profile/ChangePassword");
            }

            // If there are errors, add them to the ModelState and return the view
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); // Return the view with errors
        }
    }

}



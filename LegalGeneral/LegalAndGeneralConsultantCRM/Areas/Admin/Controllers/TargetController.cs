using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.Controllers
{

	[Area("Admin")]
    public class TargetController : Controller
    {
        private readonly LegalAndGeneralConsultantCRMContext _context;
        private readonly UserManager<LegalAndGeneralConsultantCRMUser> _userManager;

        public TargetController(LegalAndGeneralConsultantCRMContext context, UserManager<LegalAndGeneralConsultantCRMUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
             

            // Get the role ID for the "Employee" role
            var employeeRole = await _context.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Fetch all users who are in the "Employee" role
            var users = _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == employeeRole))
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}"
                }).ToList();


            // Define task types
            var taskTypes = new List<SelectListItem>
    {
        new SelectListItem { Value = "Calls", Text = "Calls" },
        new SelectListItem { Value = "ConnectedCalls", Text = "ConnectedCalls" },
        new SelectListItem { Value = "Visits", Text = "Visits" },
        new SelectListItem { Value = "Meetings", Text = "Meetings" },
        new SelectListItem { Value = "SaleAmount", Text = "Sale Amount" },
        new SelectListItem { Value = "SaleQuantity", Text = "Sale Quantity" }
    };

            // Create the view model with the users and task types
            var viewModel = new TargetTaskViewModel
            {
                Users = users,
                TaskTypes = taskTypes
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(TargetTaskViewModel viewModel)
        {

 			var user = await _userManager.GetUserAsync(User);
			var currentUser = user.Id;
			var targetTask = new TargetTask
                {
                    UserId = viewModel.SelectedUserId,
                    UserFullName = viewModel.UserFullName,
                    TaskType = viewModel.TaskType,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    createdby= currentUser,
                    targetquantity= viewModel.targetquantity,
            };

                _context.Add(targetTask);
                await _context.SaveChangesAsync();

                return RedirectToAction("Target"); // Redirect to a view that shows the saved tasks
            

           
        }

        public IActionResult Target()
        {
            var tasks = from task in _context.TargetTask
                        join user in _context.Users on task.UserId equals user.Id
                        join creator in _context.Users on task.createdby equals creator.Id // Join again to get the creator's details
                        select new
                        {
                            UserFullName = user.FirstName + " " + user.LastName,
                            TaskType = task.TaskType,
                            Description = task.Description,
                            StartDate = task.StartDate,
                            EndDate = task.EndDate,
                            TargetQuantity = task.targetquantity,
                            CreatorFullName = creator.FirstName + " " + creator.LastName // Fetch creator's full name
                        };

            var taskList = tasks.ToList().Select(t => new TargetTaskViewModel
            {
                UserFullName = t.UserFullName,
                TaskType = t.TaskType,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                targetquantity = t.TargetQuantity,
                createdby = t.CreatorFullName // Map to view model
            }).ToList();

            return View(taskList);
        }


    }
}

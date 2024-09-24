using Microsoft.AspNetCore.Identity;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models.Branches;
using Microsoft.EntityFrameworkCore;

namespace LegalAndGeneralConsultantCRM.Areas.Admin.AdminDataSeeder
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminUserAndRole(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<LegalAndGeneralConsultantCRMUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<LegalAndGeneralConsultantCRMContext>(); // Adjust to your actual DbContext

            // Create the "Admin" role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create the admin user if it doesn't exist
            var adminUser = await userManager.FindByNameAsync("admin@gmail.com");
            if (adminUser == null)
            {
                var user = new LegalAndGeneralConsultantCRMUser
                {
                    Status = true,
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    // Associate with a branch
                    Branch = new Branch
                    {
                        BranchName = "Admin Branch",
                        City = "test",
                        BranchCode = "NY001",
                        Description = "Main operational branch"
                    }
                };

                var password = "Admin123@#$";
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    
                    // Add branch data to the database if it doesn't exist
                    if (!await dbContext.Branches.AnyAsync(b => b.BranchCode == "NY001"))
                    {
                        dbContext.Branches.Add(user.Branch);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}

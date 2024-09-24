using LegalAndGeneralConsultantCRM.Areas.Admin.AdminDataSeeder;
using LegalAndGeneralConsultantCRM.Areas.Admin.SendNotificationHub;
using LegalAndGeneralConsultantCRM.Areas.Identity.Data;
using LegalAndGeneralConsultantCRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LegalAndGeneralConsultantCRMContextConnection") ?? throw new InvalidOperationException("Connection string 'LegalAndGeneralConsultantCRMContextConnection' not found.");

builder.Services.AddDbContext<LegalAndGeneralConsultantCRMContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<LegalAndGeneralConsultantCRMUser, IdentityRole>().AddDefaultTokenProviders()
   .AddEntityFrameworkStores<LegalAndGeneralConsultantCRMContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

//Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddRazorPagesOptions(options => {
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await AdminSeeder.SeedAdminUserAndRole(serviceProvider);
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Identity/Account/Login");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Map route for areas
    endpoints.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    // Map default route
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapHub<NotificationHub>("/notificationHub");

});

 
app.MapRazorPages();
app.Run();

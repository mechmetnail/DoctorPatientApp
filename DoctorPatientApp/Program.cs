using DoctorPatientApp.Data;
using DoctorPatientApp.Models;
using DoctorPatientApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

// Programs'a veri yolunun tanimi - Bu ve bir altindaki
builder.Services.AddDbContext<DataContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("SqlConnection")!;
    options.UseSqlServer(connectionString);
});


builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();

await SeedService.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

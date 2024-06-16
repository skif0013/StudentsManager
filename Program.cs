//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using MyStudentsmanager.Models;
//using System.Data;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;

//var builder = WebApplication.CreateBuilder(args);


////builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
////	.AddCookie(options => options.LoginPath = "/");

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//	.AddCookie(options =>
//	{
//		options.LoginPath = "/";
//		options.AccessDeniedPath = "/Home/MainPage";
//	});

//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
//	options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
//});
//builder.Services.AddAuthorization();


//string connection = "Server = (localdb)\\mssqllocaldb;Database = StudentsManager;Trusted_Connection=true";
//builder.Services.AddDbContext<UsersContext>(options => options.UseSqlServer(connection));

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//app.UseAuthentication();   
//app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Registr}/{id?}");

////app.MapDefaultControllerRoute();

//app.Run();


using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyStudentsmanager.Models;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Настройка аутентификации с использованием куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/";
		options.AccessDeniedPath = "/Home/MainPage";
	});

// Настройка авторизации и политик
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
	options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
});

// Настройка подключения к базе данных
string connection = "Server=(localdb)\\mssqllocaldb;Database=StudentsManager;Trusted_Connection=true";
builder.Services.AddDbContext<UsersContext>(options => options.UseSqlServer(connection));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Registr}/{id?}");

app.Run();
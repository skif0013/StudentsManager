using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStudentsmanager.Models;
using System.Security.Claims;


using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System;

namespace MyStudentsmanager.Controllers
{

	public class HomeController : Controller
	{
		private readonly UsersContext db;

		public HomeController(UsersContext dbContext)
		{
			db = dbContext;

			// Добавляем начальные данные, если база данных пустая
			if (!db.Users.Any())
			{
				var userRole = new Role { Name = "user" };
				var adminRole = new Role { Name = "admin" };

				db.Roles.AddRange(userRole, adminRole);

				var users = new List<User>
				{
					new User { Name = "Олег Васильев", Age = 26, Parol = "user1", Role = userRole },
					new User { Name = "Александр Овсов", Age = 24, Parol = "user2", Role = userRole },
					new User { Name = "Алексей Петров", Age = 25, Parol = "user3", Role = userRole },
					new User { Name = "Иван Иванов", Age = 26, Parol = "user4", Role = userRole },
					new User { Name = "Петр Андреев", Age = 23, Parol = "user5", Role = userRole },
					new User { Name = "Василий Иванов", Age = 23, Parol = "user6", Role = userRole },
					new User { Name = "Олег Кузнецов", Age = 25, Parol = "user7", Role = userRole },
					new User { Name = "Андрей Петров", Age = 24, Parol = "user8", Role = userRole },
					new User { Name = "Admin Adminov", Age = 52, Parol = "admins", Role = adminRole }
				};

				db.Users.AddRange(users);
				db.SaveChanges();
			}
		}

		public IActionResult Registr()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Registr(User user)
		{
			var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Name == user.Name && u.Parol == user.Parol);

			if (existingUser != null)
			{
				await SignInUser(existingUser);
				return RedirectToAction("MainPage");
			}
			else
			{
				if (user.Role == null)
				{
					user.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user") ?? new Role { Name = "user" };
				}


				db.Users.Add(user);
				await db.SaveChangesAsync();

				await SignInUser(user);
				return RedirectToAction("MainPage");
			}
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(User user)
		{
			Console.WriteLine($"Вход: Имя = {user.Name}, Пароль = {user.Parol}");

			var existingUser = await db.Users
				.Include(u => u.Role)
				.FirstOrDefaultAsync(u => u.Name == user.Name);

			if (existingUser == null)
			{
				Console.WriteLine("Пользователь не найден.");
				return View(user);
			}

			Console.WriteLine($"Найден пользователь: Имя = {existingUser.Name}, Пароль = {existingUser.Parol}");

			if (existingUser.Parol != user.Parol)
			{
				Console.WriteLine("Неправильное имя пользователя или пароль.");
				return View(user);
			}

			Console.WriteLine($"Найден пользователь: Имя = {existingUser.Name}, Пароль = {existingUser.Parol}");
			await SignInUser(existingUser);
			return RedirectToAction("MainPage");
		}

		[Authorize(Roles = "admin, user")]
		public async Task<IActionResult> MainPage()
		{
			return View(await db.Users.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> Delete()
		{
			var userName = Request.Cookies["name"];
			if (!string.IsNullOrEmpty(userName))
			{
				var user = await db.Users.FirstOrDefaultAsync(u => u.Name == userName);
				if (user != null)
				{
					db.Users.Remove(user);
					await db.SaveChangesAsync();

					await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
					Response.Cookies.Delete("name");
					Response.Cookies.Delete("id");

					return RedirectToAction("Registr");
				}
			}

			return NotFound();
		}

		[Authorize(Roles = "admin, user")]
		public async Task<IActionResult> Edit()
		{
			var userId = Request.Cookies["id"];
			if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
			{
				return BadRequest("Идентификатор пользователя отсутствует в куках.");
			}

			var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userIdInt);
			if (user == null)
			{
				return NotFound("Пользователь не найден.");
			}

			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(User user)
		{
			if (user == null)
			{
				return BadRequest("Некорректные данные пользователя.");
			}

			var existingUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
			if (existingUser == null)
			{
				return NotFound("Пользователь не найден.");
			}

			existingUser.Name = user.Name;
			existingUser.Age = user.Age;
			existingUser.Parol = user.Parol;

			if (existingUser.Role == null)
			{
				var defaultRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user") ?? new Role { Name = "user" };
				existingUser.Role = defaultRole;
			}

			db.Users.Update(existingUser);
			await db.SaveChangesAsync();

			

			return RedirectToAction("MainPage");
		}

		[Authorize(Roles = "admin")]
		public async Task<IActionResult> AdminPage()
		{
			var users = await db.Users.Include(u => u.Role).ToListAsync();
			return View(users);
		}

		[HttpPost]
		public async Task<IActionResult> AdminDelete(int? id)
		{
			if (id == null)
			{
				return BadRequest("Некорректный идентификатор пользователя.");
			}

			var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
			if (user != null)
			{
				db.Users.Remove(user);
				await db.SaveChangesAsync();
				return RedirectToAction("AdminPage");
			}

			return NotFound();
		}

		[Authorize(Roles = "admin")]
		public async Task<IActionResult> AdminEdit(int? id)
		{
			if (id == null)
			{
				return BadRequest("Некорректный идентификатор пользователя.");
			}

			var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
			if (user == null)
			{
				return NotFound("Пользователь не найден.");
			}

			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> AdminEdit(User user)
		{
			if (user == null)
			{
				return BadRequest("Некорректные данные пользователя.");
			}

			var existingUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
			if (existingUser == null)
			{
				return NotFound("Пользователь не найден.");
			}

			existingUser.Name = user.Name;
			existingUser.Age = user.Age;
			existingUser.Parol = user.Parol;

			// Обновление роли, если она отличается
			if (user.Role != null && user.Role.Name != existingUser.Role?.Name)
			{
				var newRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == user.Role.Name) ?? new Role { Name = user.Role.Name };
				existingUser.Role = newRole;
			}

			db.Users.Update(existingUser);
			await db.SaveChangesAsync();

			return RedirectToAction("AdminPage");
		}

		private async Task SignInUser(User user)
		{
			var userWithRole = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
			if (userWithRole == null || userWithRole.Role == null || string.IsNullOrEmpty(userWithRole.Role.Name))
			{
				throw new InvalidOperationException("User role is not properly assigned.");
			}


			if (userWithRole == null)
			{
				Results.Unauthorized();
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userWithRole!.Name!),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, userWithRole.Role.Name)
			};
			Response.Cookies.Append("name", userWithRole!.Name!);
			Response.Cookies.Append("id", user.Id.ToString());
			var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			await HttpContext.SignInAsync(claimsPrincipal);


		}
	}
}
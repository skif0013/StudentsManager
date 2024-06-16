//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MyStudentsmanager.Models;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using System.Net.Http;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using System;

//namespace MyStudentsmanager.Controllers
//{
//	public class HomeController : Controller
//	{

//		private readonly UsersContext db;

//		public HomeController(UsersContext dbContext)
//		{
//			db = dbContext;


//			var userRole = new Role { Name = "user" };
//			var adminRole = new Role { Name = "admin" };

//			//добавляем начальные данные
//			if (!db.Users.Any())
//			{


//				User user1 = new User { Name = "Олег Васильев", Age = 26, Parol = "user1", Role = userRole };
//				User user2 = new User { Name = "Александр Овсов", Age = 24, Parol = "user2", Role = userRole };
//				User user3 = new User { Name = "Алексей Петров", Age = 25, Parol = "user3", Role = userRole };
//				User user4 = new User { Name = "Иван Иванов", Age = 26, Parol = "user4", Role = userRole };
//				User user5 = new User { Name = "Петр Андреев", Age = 23, Parol = "user5", Role = userRole };
//				User user6 = new User { Name = "Василий Иванов", Age = 23, Parol = "user6", Role = userRole };
//				User user7 = new User { Name = "Олег Кузнецов", Age = 25, Parol = "user7", Role = userRole };
//				User user8 = new User { Name = "Андрей Петров", Age = 24, Parol = "user8", Role = userRole };
//				User admin1 = new User { Name = "Admin Adminov", Age = 52, Parol = "admins", Role = adminRole };

//				db.Roles.AddRange(userRole, adminRole);
//				db.Users.AddRange(user1, user2, user3, user4, user5, user6, user7, user8);
//				db.SaveChanges();
//			}
//		}

//		public IActionResult Registr()
//		{
//			return View();
//		}

//		[HttpPost]
//		public async Task<IActionResult> Registr(User user /*string name, string parol, string age*/)
//		{
//			//var existingUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Name == user.Name && u.Parol == user.Parol);

//			//if (existingUser != null)
//			//{
//			//	// Пользователь найден, проверка наличия роли
//			//	if (existingUser.Role == null)
//			//	{
//			//		existingUser.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");

//			//		// Если роль "user" не найдена в базе данных, создаем новую роль
//			//		if (existingUser.Role == null)
//			//		{
//			//			existingUser.Role = new Role { Name = "user" };
//			//		}
//			//	}

//			//	// Установка всех необходимых cookies и вход пользователя
//			//	await SignInUser(existingUser);
//			//	return RedirectToAction("MainPage");
//			//}
//			//else
//			//{
//			//	// Если пользователь не найден, создаем нового пользователя и назначаем ему роль "user"
//			//	user.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");

//			//	// Если роль "user" не найдена в базе данных, создаем новую роль
//			//	if (user.Role == null)
//			//	{
//			//		user.Role = new Role { Name = "user" };
//			//	}

//			//	// Добавление нового пользователя в базу данных
//			//	db.Users.Add(user);
//			//	await db.SaveChangesAsync();

//			//	// Установка всех необходимых cookies и вход пользователя
//			//	await SignInUser(user);
//			//	return RedirectToAction("MainPage");
//			//}


//			var existingUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Name == user.Name && u.Parol == user.Parol);

//			if (existingUser != null)
//			{
//				if (existingUser.Role == null)
//				{
//					existingUser.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "admin") ?? new Role { Name = "admin" };
//				}

//				await SignInUser(existingUser);
//				return RedirectToAction("MainPage");
//			}
//			else
//			{
//				user.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "admin") ?? new Role { Name = "admin" };
//				db.Users.Add(user);
//				await db.SaveChangesAsync();

//				await SignInUser(user);
//				return RedirectToAction("MainPage");
//			}
//		}

//		public IActionResult Login()
//		{
//			return View();
//		}

//		[HttpPost]
//		public async Task<IActionResult> Login(User user)
//		{
//			if (user == null)
//			{
//				return BadRequest("Вы неправильно заполнили поля");
//			}

//			if (user.Role == null)
//			{
//				var defaultRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
//				if (defaultRole == null)
//				{
//					defaultRole = new Role { Name = "user" };
//					db.Roles.Add(defaultRole);
//					await db.SaveChangesAsync();
//				}
//				user.RoleId = defaultRole.Id;
//			}



//			if (CheckUserExists(user))
//			{
//				await SignInUser(user);
//				return RedirectToAction("MainPage");
//			}



//			await db.SaveChangesAsync();
//			// установка всех необходимых cookies
//			await SignInUser(user);
//			return RedirectToAction("MainPage");
//		}

//		[Authorize(Roles = "admin, user")]
//		public async Task<IActionResult> MainPage()
//		{
//			return View(await db.Users.ToListAsync());
//		}

//		[HttpPost]
//		public async Task<IActionResult> Delete()
//		{

//			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//			var user = await db.Users.FirstOrDefaultAsync(u => u.Name == Request.Cookies["name"]);
//			Response.Cookies.Delete("name");
//			Response.Cookies.Delete("id");
//			if (user != null)
//			{
//				db.Users.Remove(user);
//				await db.SaveChangesAsync();
//				return RedirectToAction("Registr");
//			}


//			return NotFound();
//		}

//		[Authorize(Roles = "user, admin")]
//		public async Task<IActionResult> Edit()
//		{
//			var userId = Request.Cookies["id"];
//			if (string.IsNullOrEmpty(userId))
//			{
//				return BadRequest("Идентификатор пользователя отсутствует в куках.");
//			}

//			// Преобразовать значение из строки в целое число
//			if (!int.TryParse(userId, out int userId_))
//			{
//				return BadRequest("Некорректный идентификатор пользователя в куках.");
//			}



//			// Ищем пользователя в базе данных по идентификатору
//			var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId_);

//			// Проверяем, найден ли пользователь
//			if (user == null)
//			{
//				return NotFound("Пользователь не найден.");
//			}

//			// Возвращаем представление с данными пользователя
//			return View(user);

//			//// Ищем пользователя в базе данных по имени из куки
//			//var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

//			//// Проверяем, найден ли пользователь
//			//if (user == null)
//			//{
//			//	return NotFound("Пользователь не найден.");
//			//}

//			//// Проверяем, есть ли у пользователя роль, если нет - создаем новую роль
//			//if (user.Role == null)
//			//{
//			//	user.Role = new Role();
//			//}

//			//// Возвращаем представление с данными пользователя
//			//return View(user);
//		}

//		[HttpPost]
//		public async Task<IActionResult> Edit(User user, Role role)
//		{
//			if (user == null)
//			{
//				return BadRequest("Некорректные данные пользователя.");
//			}
//			;
//			// Найти существующего пользователя в базе данных по Id
//			var existingUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
//			if (existingUser == null)
//			{
//				return NotFound("Пользователь не найден.");
//			}

//			// Обновить свойства существующего пользователя значениями из формы
//			existingUser.Name = user.Name;
//			existingUser.Age = user.Age;
//			existingUser.Parol = user.Parol;

//			// Присвоить роль (на сервере, пользователи не могут изменить роль через форму)
//			if (existingUser.Role == null)
//			{
//				// Если у пользователя нет роли, назначить роль по умолчанию (например, "user")
//				var defaultRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
//				if (defaultRole == null)
//				{
//					defaultRole = new Role { Name = "user" };
//					db.Roles.Add(defaultRole);
//					await db.SaveChangesAsync();
//				}
//				existingUser.RoleId = defaultRole.Id;
//			}

//			// Обновить пользователя в базе данных
//			db.Users.Update(existingUser);
//			await db.SaveChangesAsync();

//			return RedirectToAction("MainPage");
//		}


//		[Authorize(Roles = "admin")]
//		public async Task<IActionResult> AdminPage()
//		{
//			var users = await db.Users.Include(u => u.Role).ToListAsync();
//			return View(users);
//		}

//		[HttpPost]
//		public async Task<IActionResult> AdminDelete(int? id)
//		{

//			var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
//			if (user != null)
//			{
//				db.Users.Remove(user);
//				await db.SaveChangesAsync();
//				return RedirectToAction("AdminPage");
//			}


//			return NotFound();
//		}


//		[Authorize(Roles = "admin")]
//		public async Task<IActionResult> AdminEdit(int? id)
//		{

//			if (id == null)
//			{
//				return BadRequest("Некорректный идентификатор пользователя.");
//			}

//			// Ищем пользователя в базе данных по идентификатору
//			var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

//			// Проверяем, найден ли пользователь
//			if (user == null)
//			{
//				return NotFound("Пользователь не найден.");
//			}

//			// Возвращаем представление с данными пользователя
//			return View(user);

//			//// Ищем пользователя в базе данных по имени из куки
//			//var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

//			//// Проверяем, найден ли пользователь
//			//if (user == null)
//			//{
//			//	return NotFound("Пользователь не найден.");
//			//}

//			//// Проверяем, есть ли у пользователя роль, если нет - создаем новую роль
//			//if (user.Role == null)
//			//{
//			//	user.Role = new Role();
//			//}

//			//// Возвращаем представление с данными пользователя
//			//return View(user);
//		}

//		[HttpPost]
//		public async Task<IActionResult> AdminEdit(User user)
//		{

//			if (user == null)
//			{
//				return BadRequest("Некорректные данные пользователя.");
//			}

//			// Проверка, что пользовательский объект содержит роль
//			if (user.Role == null)
//			{
//				return BadRequest("Роль пользователя не указана.");
//			}

//			// Найти существующую роль по имени
//			var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == user.Role.Name);

//			// Если роль не найдена, создать новую
//			if (role == null)
//			{
//				role = new Role { Name = user.Role.Name };
//				db.Roles.Add(role);
//				await db.SaveChangesAsync(); // Сохранить новую роль и получить её Id
//			}

//			// Присвоить пользователю Id существующей или новой роли
//			user.RoleId = role.Id;
//			user.Role = null; // Очистить навигационное свойство, чтобы избежать дублирования

//			// Обновить пользователя в базе данных
//			db.Users.Update(user);
//			await db.SaveChangesAsync();

//			return RedirectToAction("AdminPage");

//			//user.Role = new Role();
//			//db.Users.Update(user);
//			//await db.SaveChangesAsync();
//			//return RedirectToAction("AdminPage");
//		}


//		private bool CheckUserExists(User user)
//		{
//			return db.Users.Any(u => u.Name == user.Name && u.Parol == user.Parol);
//		}
//		//private async Task SignInUser(User user)
//		//{
//		//	var userWithRole = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
//		//	if (userWithRole == null || userWithRole.Role == null || string.IsNullOrEmpty(userWithRole.Role.Name))
//		//	{
//		//		throw new InvalidOperationException("User role is not properly assigned.");
//		//	}


//		//	var claims = new List<Claim>
//		//	{
//		//		new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
//		//		new Claim(ClaimsIdentity.DefaultRoleClaimType, userWithRole.Role.Name)
//		//	};

//		//	Response.Cookies.Append("name",userWithRole.Name);
//		//	Response.Cookies.Append("id", userWithRole.Id.ToString());
//		//	var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
//		//	var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//		//	await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
//		//}

//		private async Task SignInUser(User user)
//		{
//			var userWithRole = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
//			if (userWithRole == null || userWithRole.Role == null || string.IsNullOrEmpty(userWithRole.Role.Name))
//			{
//				throw new InvalidOperationException("User role is not properly assigned.");
//			}

//			var claims = new List<Claim>
//			{
//				new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
//				new Claim(ClaimsIdentity.DefaultRoleClaimType, userWithRole.Role.Name)
//			};

//			Response.Cookies.Append("name", userWithRole.Name);
//			Response.Cookies.Append("id", userWithRole.Id.ToString());
//			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
//		}

//	}
//}


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
				if(user.Role == null)
				{
					user.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user") ?? new Role { Name = "user" };
				}
				
				//user.Role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "admin") ?? new Role { Name = "admin" };
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

			var existingUser = await db.Users
					.Include(u => u.Role) // Включаем данные о роли
					.FirstOrDefaultAsync(u => u.Name == user.Name && u.Parol == user.Parol);

			if(existingUser == null)
			{
				return NotFound();
			}

			if (existingUser != null)
			{
				await SignInUser(existingUser);

				// Логирование для отладки
				Console.WriteLine($"{existingUser.Name} {existingUser.Role.Name}");

				return RedirectToAction("MainPage");
			}

			// Обработка случая, когда пользователь не найден
			Console.WriteLine($"\t\n hii  {user.Name} {existingUser.Age}");
			return View(user);

			// Логирование для отладки
			//Console.WriteLine($"{user.Id} {user.Name} {user.Parol} ");


			//return View(user);


			//var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Name == user.Name && u.Parol == user.Parol);

			//if (existingUser != null)
			//{
			//	await SignInUser(existingUser);
			//	return RedirectToAction("MainPage");
			//}


			//return View(user);
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

			//var claims = new List<Claim>
			//{
			//	new Claim(ClaimsIdentity.DefaultNameClaimType, userWithRole.Name),
			//	new Claim(ClaimsIdentity.DefaultRoleClaimType, userWithRole.Role.Name)
			//};

			//var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

			//await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

			//Response.Cookies.Append("name", user.Name);
			//Response.Cookies.Append("id", user.Id.ToString());
		}
	}
}
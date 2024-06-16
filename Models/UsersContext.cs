using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyStudentsmanager.Models
{
	public class UsersContext : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;
		public DbSet<Role> Roles { get; set; } = null!;
		public UsersContext(DbContextOptions<UsersContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}
	}
}




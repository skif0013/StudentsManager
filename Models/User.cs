namespace MyStudentsmanager.Models
{
	public class User
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public int Age { get; set; }
		public string? Parol { get; set; }
		public int RoleId { get; set; }
		public Role? Role { get; set; }
	}



}

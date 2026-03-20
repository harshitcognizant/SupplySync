namespace SupplySync.DTOs.User
{

	public class LoginResponseDto
	{
		public string Token { get; set; } = default!;
		public DateTime ExpiresAtUtc { get; set; }
		public int UserID { get; set; }
		public string Name { get; set; } = default!;
		public string Email { get; set; } = default!;
		public List<string> Roles { get; set; } = new();
	}

}

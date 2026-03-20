namespace SupplySync.DTOs.User
{
		public class UserListResponseDto
		{
			public List<UserResponseDto> Items { get; set; } = new();

			// Basic pagination metadata
			public int PageNumber { get; set; }
			public int PageSize { get; set; }
			public int TotalCount { get; set; }
		}
}

using Microsoft.EntityFrameworkCore;

namespace SupplySync.Config
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
		}


	}
}

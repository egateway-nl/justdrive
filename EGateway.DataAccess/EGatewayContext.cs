using EGateway.Model;
using Microsoft.EntityFrameworkCore;

namespace EGateway.DataAccess
{
	public class EGatewayContext : DbContext
	{
		public EGatewayContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

		public DbSet<Driver>? Drivers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Driver>().HasData(new List<Driver>
			{
				new Driver
				{
					Id = 1,
					Eori = "EU.EORI.BUDR0000001"
				},
				new Driver
				{
					Id = 2,
					Eori = "EU.EORI.BUDR0000002"
				}
			});
		}
	}
}
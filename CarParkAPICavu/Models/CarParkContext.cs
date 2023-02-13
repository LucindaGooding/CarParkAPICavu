using Microsoft.EntityFrameworkCore;

namespace CarParkAPICavu.Models
{
    public class CarParkContext : DbContext
    {
        public CarParkContext(DbContextOptions<CarParkContext> options) 
            : base(options)
        {
        }

        public DbSet<CarPark> Cars { get; set; } = null!;
    }
}

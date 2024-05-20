using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SimpleExampleUsingRedis.Models
{
    public class CarContext : DbContext
    {
        public DbSet<Car> Cars { get; set; } = null!;
        public CarContext(DbContextOptions<CarContext> options)
            : base(options)
        {

        }
    }
}

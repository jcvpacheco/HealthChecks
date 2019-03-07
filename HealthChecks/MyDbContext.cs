using Microsoft.EntityFrameworkCore;

namespace HealthChecks
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
          : base(options)
        { }
    }
}
    
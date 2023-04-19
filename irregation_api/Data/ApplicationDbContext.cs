using irregation_api.Entity;
using Microsoft.EntityFrameworkCore;

namespace irregation_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {   
        }

        public DbSet<SensorEntity> Sensors { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<IrregationScheduleEntity> IrregationSchedules { get; set; }
    }
}

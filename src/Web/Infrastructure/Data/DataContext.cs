using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;

namespace Web.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<StaticSensorReading> StaticSensorReadings { get; set; }

        public virtual DbSet<Sensor> Sensors { get; set; }

        public virtual DbSet<StaticSensor> StaticSensors { get; set; }

        public virtual DbSet<PortableSensor> PortableSensors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sensor>().HasKey(x => x.Id);
            modelBuilder.Entity<Sensor>().Property(z => z.Id).ValueGeneratedOnAdd();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}

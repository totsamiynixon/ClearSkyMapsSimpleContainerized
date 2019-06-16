using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Data.Models;
using Web.Data.Models.Identity;

namespace Web.Data
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
    }
}

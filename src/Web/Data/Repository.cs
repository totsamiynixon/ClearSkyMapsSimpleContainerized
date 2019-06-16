using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Data.Models.Identity;
using Z.EntityFramework.Plus;

namespace Web.Data
{
    public class Repository : IRepository
    {

        private static readonly object Lock = new object();
        private static bool _dbInitialized;

        private readonly IServiceProvider _serviceProvider;
        public Repository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<StaticSensor> AddStaticSensorAsync(string ipAddress, double latitude, double longitude)
        {
            using (var _context = GetFreshContext())
            {
                var sensor = new StaticSensor
                {
                    Readings = new List<StaticSensorReading>(),
                    IPAddress = ipAddress,
                };
                sensor.Latitude = latitude;
                sensor.Longitude = longitude;
                _context.StaticSensors.Add(sensor);
                await _context.SaveChangesAsync();
                return sensor;
            };
        }

        public async Task<PortableSensor> AddPortableSensorAsync(string ipAddress)
        {
            using (var _context = GetFreshContext())
            {
                var sensor = new PortableSensor
                {
                    IPAddress = ipAddress
                };
                _context.PortableSensors.Add(sensor);
                await _context.SaveChangesAsync();
                return sensor;
            }
        }

        public async Task<StaticSensor> UpdateStaticSensorCoordinatesAsync(int id, double latitude, double longitude)
        {
            using (var _context = GetFreshContext())
            {
                var staticSensor = await _context.StaticSensors.FirstOrDefaultAsync(f => f.Id == id);
                staticSensor.Latitude = latitude;
                staticSensor.Longitude = longitude;
                _context.Entry(staticSensor).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return staticSensor;
            }
        }

        public async Task<StaticSensor> UpdateStaticSensorVisibilityAsync(int id, bool value)
        {
            using (var _context = GetFreshContext())
            {
                var staticSensor = await _context.StaticSensors.FirstOrDefaultAsync(f => f.Id == id);
                staticSensor.IsVisible = value;
                _context.Entry(staticSensor).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return staticSensor;
            }
        }

        public async Task<Sensor> ChangeSensorActivationAsync(int id, bool value)
        {
            using (var _context = GetFreshContext())
            {
                var sensor = await _context.Sensors.FirstOrDefaultAsync(f => f.Id == id);
                sensor.IsActive = value;
                _context.Entry(sensor).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return sensor;
            }
        }

        public async Task<List<Sensor>> GetSensorsAsync()
        {
            using (var context = GetFreshContext())
            {
                var query = context
                      .Sensors.AsNoTracking().Where(f => !f.IsDeleted);
                var sensors = await query.ToListAsync();
                return sensors;
            }
        }

        public async Task<List<StaticSensor>> GetStaticSensorsForCacheAsync()
        {
            using (var context = GetFreshContext())
            {
                var sensors = await context
                      .StaticSensors.AsNoTracking().Where(f => f.IsActive && f.IsVisible && !f.IsDeleted).Select(f => new StaticSensor
                      {
                          Id = f.Id,
                          IPAddress = f.IPAddress,
                          IsActive = f.IsActive,
                          IsDeleted = f.IsDeleted,
                          IsVisible = f.IsVisible,
                          Latitude = f.Latitude,
                          Longitude = f.Longitude,
                          Readings = f.Readings
                                            .OrderByDescending(s => s.Created)
                                            .Take(10).ToList()
                      }).ToListAsync();
                return sensors;
            }
        }

        public async Task<List<StaticSensor>> GetStaticSensorsAsync(bool withReadings)
        {
            using (var context = GetFreshContext())
            {
                var query = context
                      .StaticSensors.AsNoTracking().Where(f => !f.IsDeleted);
                if (withReadings)
                {
                    query = query.Select(f => new StaticSensor
                    {
                        Id = f.Id,
                        IPAddress = f.IPAddress,
                        IsActive = f.IsActive,
                        IsDeleted = f.IsDeleted,
                        IsVisible = f.IsVisible,
                        Latitude = f.Latitude,
                        Longitude = f.Longitude,
                        Readings = f.Readings
                                            .OrderByDescending(s => s.Created)
                                            .Take(10).ToList()
                    });
                }
                var sensors = await query.ToListAsync();
                return sensors;
            }
        }

        public async Task<Sensor> GetSensorByIdAsync(int id)
        {
            using (var context = GetFreshContext())
            {
                return await context.Sensors.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            }
        }

        public async Task<StaticSensor> GetStaticSensorByIdAsync(int id, bool withReadings = false)
        {
            using (var context = GetFreshContext())
            {
                var query = context
                     .StaticSensors.AsNoTracking().AsQueryable();
                if (withReadings)
                {
                    query = query.Select(f => new StaticSensor
                    {
                        Id = f.Id,
                        IPAddress = f.IPAddress,
                        IsActive = f.IsActive,
                        IsDeleted = f.IsDeleted,
                        IsVisible = f.IsVisible,
                        Latitude = f.Latitude,
                        Longitude = f.Longitude,
                        Readings = f.Readings
                                            .OrderByDescending(s => s.Created)
                                            .Take(10).ToList()
                    });
                }
                return await query.FirstOrDefaultAsync(f => f.Id == id);
            }
        }

        public async Task DeleteSensorAsync(int id)
        {
            using (var _context = GetFreshContext())
            {
                await _context.Sensors.Where(f => f.Id == id).UpdateAsync(f => new StaticSensor
                {
                    IsDeleted = true
                });
            }
        }

        public async Task RemoveSensorsFromDatabaseAsync(params int[] ids)
        {
            using (var _context = GetFreshContext())
            {
                await _context.Sensors.Where(f => ids.Any(s => s == f.Id)).DeleteAsync();
            }
        }

        public async Task RemoveAllSensorsFromDatabaseAsync()
        {
            using (var _context = GetFreshContext())
            {
                await _context.Sensors.Where(f => true).DeleteAsync();
            }
        }

        public async Task<StaticSensorReading> AddReadingAsync(StaticSensorReading reading)
        {
            if (reading.StaticSensorId <= 0)
            {
                throw new ArgumentException("SensorId should be provided in reading!", nameof(reading.StaticSensorId));
            }
            using (var _context = GetFreshContext())
            {
                _context.StaticSensorReadings.Add(reading);
                await _context.SaveChangesAsync();
                return reading;
            }
        }

        public void ReinitializeDb()
        {
            lock (Lock)
            {
                using (var _context = GetFreshContext())
                {
                    try
                    {
                        _context.Database.Migrate();

                    }
                    catch (SqlException exception) when (exception.Number == 1801)
                    {
                        // retry
                    }
                }

                using (var _context = GetFreshIdentityDbContext())
                {
                    try
                    {
                        _context.Database.Migrate();
                        Seed(_context);

                    }
                    catch (SqlException exception) when (exception.Number == 1801)
                    {
                        // retry
                    }
                }
            }
        }

        private DataContext GetFreshContext()
        {
            return (DataContext)_serviceProvider.GetService(typeof(DataContext));
        }

        private IdentityDbContext GetFreshIdentityDbContext()
        {
            return (IdentityDbContext)_serviceProvider.GetService(typeof(IdentityDbContext));
        }

        private void Seed(IdentityDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(new IdentityRole
                {
                    Name = "Admin"
                });

                var supervisorRole = new IdentityRole
                {
                    Name = "Supervisor"

                };
                context.Roles.Add(supervisorRole);

                var hasher = new PasswordHasher<User>();
                var userAdmin = new User
                {
                    Email = "supervisor@clearskymaps.com",
                    NormalizedEmail = "SUPERVISOR@CLEARSKYMAPS.COM",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "supervisor@clearskymaps.com",
                    NormalizedUserName = "SUPERVISOR@CLEARSKYMAPS.COM",
                    IsActive = true
                };
                context.Users.Add(userAdmin);
                context.SaveChanges();

                userAdmin.PasswordHash = hasher.HashPassword(userAdmin, "VerySecurePassword");
                context.Entry(userAdmin).State = EntityState.Modified;
                context.SaveChanges();

                var userRoleSet = context.Set<IdentityUserRole<string>>();
                userRoleSet.Add(new IdentityUserRole<string>
                {
                    RoleId = supervisorRole.Id,
                    UserId = userAdmin.Id
                });
                context.SaveChanges();
            }
        }
    }
}

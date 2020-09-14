using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Application.Readings.Queries.DTO;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Queries
{
    public class ReadingsQueries : IReadingsQueries
    {
        private readonly IMapper _mapper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public ReadingsQueries(
            IDataContextFactory<DataContext> dataContextFactory, IMapper mapper)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<SensorDTO>> GetSensorsAsync()
        {
            await using var context = _dataContextFactory.Create();
            var query = context
                .Set<Sensor>().AsNoTracking().Where(f => !f.IsDeleted);
            var sensors = await query.ToListAsync();
            return _mapper.Map<List<Sensor>, List<SensorDTO>>(sensors);
        }

        public async Task<SensorDTO> GetSensorByIdAsync(int id)
        {
            await using var context = _dataContextFactory.Create();
            return _mapper.Map<Sensor, SensorDTO>(await context.Sensors.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id));
        }

        public async Task<StaticSensorDTO> GetStaticSensorByIdAsync(int id)
        {
            await using var context = _dataContextFactory.Create();
            var query = context
                .StaticSensors.AsNoTracking().AsQueryable();
            var staticSensor = await query.FirstOrDefaultAsync(f => f.Id == id);
            return _mapper.Map<StaticSensor, StaticSensorDTO>(staticSensor);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.PWA.Application.Readings.Queries;
using Web.Areas.PWA.Application.Readings.Queries.DTO;
using Web.Areas.PWA.Models.API.Sensors;

namespace Web.Areas.PWA.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<StaticSensorDTO, StaticSensorModel>();
            x.CreateMap<StaticSensorReadingDTO, StaticSensorReadingModel>();
        }));
        
        private readonly IReadingsQueries _readingsQueries;

        public SensorsController(IReadingsQueries readingsQueries)
        {
            _readingsQueries = readingsQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var sensors = await _readingsQueries.GetStaticSensorsAsync();
            var model = _mapper.Map<List<StaticSensorDTO>, List<StaticSensorModel>>(sensors);
            return Ok(model);
        }
    }
}
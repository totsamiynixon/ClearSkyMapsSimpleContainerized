using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Readings.Queries;
using Web.Application.Readings.Queries.DTO;
using Web.Models.API.Sensors;

namespace Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReadingsQueries _readingsQueries;

        public SensorsController(IReadingsQueries readingsQueries, IMapper mapper)
        {
            _readingsQueries = readingsQueries ?? throw new ArgumentNullException(nameof(readingsQueries));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
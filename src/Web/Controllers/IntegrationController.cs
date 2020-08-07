using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Readings.Commands;
using Web.Application.Readings.DTO;
using Web.Application.Readings.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IntegrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("getaspost")]
        public async Task<IActionResult> GetAsPostDataAsync(string data)
        {
            var model = GetModelFromString(data);
            if (model.apiKey == null || model.reading == null)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                await _mediator.Send(new CreateReadingCommand(model.reading, model.apiKey));
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }


        private (string apiKey,SensorReadingDTO reading) GetModelFromString(string data)
        {
            try
            {
                var trimmed = data.Trim(';');
                var groupes = trimmed.Split(",").Select(x => x.Replace('.', ',')).ToArray();
                return (apiKey: groupes[0], reading: new SensorReadingDTO
                {
                    Temp = float.Parse(groupes[1]),
                    Hum = float.Parse(groupes[2]),
                    Preassure = float.Parse(groupes[3]),
                    CO2 = float.Parse(groupes[4]),
                    LPG = float.Parse(groupes[5]),
                    CO = float.Parse(groupes[6]),
                    CH4 = float.Parse(groupes[7]),
                    Dust = float.Parse(groupes[8]),
                    Longitude = float.Parse(groupes[9]),
                    Latitude = float.Parse(groupes[10])
                });
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }
    }
}
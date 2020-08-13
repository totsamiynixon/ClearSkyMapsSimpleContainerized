using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.DTO;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Sensors;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflict
    [Route( AdminArea.APIRoutePrefix + "/sensors")]
    [ApiController]
    public class APISensorsController : Controller
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorDTO, SensorListItemModel>();
            x.CreateMap<StaticSensorDTO, StaticSensorListItemModel>()
                .IncludeBase<SensorDTO, SensorListItemModel>();

            x.CreateMap<CreateStaticSensorModel, CreateStaticSensorCommand>()
                .ConstructUsing(z => new CreateStaticSensorCommand(z.ApiKey, z.Latitude, z.Longitude));
            x.CreateMap<CreatePortableSensorModel, CreatePortableSensorCommand>()
                .ConstructUsing(z => new CreatePortableSensorCommand(z.ApiKey));
            x.CreateMap<DeleteSensorModel, DeleteSensorCommand>()
                .ConstructUsing(z => new DeleteSensorCommand(z.Id.Value, z.IsCompletely));
            x.CreateMap<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>()
                .ConstructUsing(z => new ChangeSensorActivationStateCommand(z.Id.Value, z.IsActive));
            x.CreateMap<ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>()
                .ConstructUsing(z => new ChangeStaticSensorVisibilityStateCommand(z.Id.Value, z.IsVisible));
            
            x.CreateMap<SensorDTO, SensorModel>();
            x.CreateMap<StaticSensorDTO, StaticSensorModel>()
                .IncludeBase<SensorDTO, SensorModel>();
            x.CreateMap<PortableSensorDTO, PortableSensorModel>()
                .IncludeBase<SensorDTO, SensorModel>();
        }));


        private readonly IReadingsQueries _readingsQueries;
        private readonly IMediator _mediator;

        public APISensorsController(IReadingsQueries readingsQueries, IMediator mediator)
        {
            _readingsQueries = readingsQueries;
            _mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var sensors = await _readingsQueries.GetSensorsAsync();
            var model = new
            {
                PortableSensors =
                    _mapper.Map<List<PortableSensorDTO>, List<SensorListItemModel>>(sensors
                        .OfType<PortableSensorDTO>()
                        .ToList()),
                StaticSensors =
                    _mapper.Map<List<StaticSensorDTO>, List<StaticSensorListItemModel>>(sensors
                        .OfType<StaticSensorDTO>()
                        .ToList()),
            };
            return Ok(model);
        }


        [HttpPost]
        public async Task<ActionResult> CreateStaticSensor(
            CreateStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            var command = _mapper.Map<CreateStaticSensorModel, CreateStaticSensorCommand>(model);
            var sensorDto = await _mediator.Send(command);

            return Ok(_mapper.Map<StaticSensorDTO, StaticSensorModel>(sensorDto));
        }

        [HttpPost]
        public async Task<ActionResult> CreatePortableSensor(
            CreatePortableSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            var command = _mapper.Map<CreatePortableSensorModel, CreatePortableSensorCommand>(model);
            var portableSensorDto = await _mediator.Send(command);

            return Ok(_mapper.Map<PortableSensorDTO, PortableSensorModel>(portableSensorDto));
        }


        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(
            DeleteSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            try
            {
                var command = _mapper.Map<DeleteSensorModel, DeleteSensorCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeActivation(
            ChangeActivationSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            try
            {
                var command = _mapper.Map<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(
            ChangeVisibilityStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            try
            {
                var command =
                    _mapper.Map<ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex);
            }

            return Ok();
        }
    }
}
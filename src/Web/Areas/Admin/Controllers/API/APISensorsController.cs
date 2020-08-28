using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Commands.DTO;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Sensors;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflict
    [Route(AdminArea.APIRoutePrefix + "/sensors")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class APISensorsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IReadingsQueries _readingsQueries;
        private readonly IMediator _mediator;

        public APISensorsController(IReadingsQueries readingsQueries, IMediator mediator, IMapper mapper)
        {
            _readingsQueries = readingsQueries ?? throw new ArgumentNullException(nameof(readingsQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Returns all sensors
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(SensorListModel), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllSensors()
        {
            var sensors = await _readingsQueries.GetSensorsAsync();
            var model = new SensorListModel
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

        /// <summary>
        /// Create static sensor
        /// </summary>
        /// <response code="400">If model is invalid</response>
        [HttpPost("static")]
        [ProducesResponseType(typeof(StaticSensorModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateStaticSensor(
            CreateStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            var command = _mapper.Map<CreateStaticSensorModel, CreateStaticSensorCommand>(model);
            var sensorDto = await _mediator.Send(command);

            return Ok(_mapper.Map<StaticSensorDTO, StaticSensorModel>(sensorDto));
        }

        /// <summary>
        /// Create portable sensor
        /// </summary>
        /// <response code="400">If model is invalid</response>
        [HttpPost("portable")]
        [ProducesResponseType(typeof(PortableSensorModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreatePortableSensor(
            CreatePortableSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var command = _mapper.Map<CreatePortableSensorModel, CreatePortableSensorCommand>(model);
            var portableSensorDto = await _mediator.Send(command);

            return Ok(_mapper.Map<PortableSensorDTO, PortableSensorModel>(portableSensorDto));
        }


        /// <summary>
        /// Delete sensor
        /// </summary>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If sensor not found</response>
        [Authorize(Policy = AuthPolicies.Supervisor)]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(
            DeleteSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                var command = _mapper.Map<DeleteSensorModel, DeleteSensorCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Change sensor activation state
        /// </summary>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If sensor not found</response>
        [HttpPost("changeActivation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeActivation(
            ChangeActivationSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                var command = _mapper.Map<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Change static sensor visibility state
        /// </summary>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If static sensor not found</response>
        [HttpPost("static/changeVisibility")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(
            ChangeVisibilityStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                var command =
                    _mapper.Map<ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }
    }
}
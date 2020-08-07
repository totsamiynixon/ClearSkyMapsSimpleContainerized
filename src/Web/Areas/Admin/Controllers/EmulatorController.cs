using System.Threading.Tasks;
using Web.Areas.Admin.Models.Emulator;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Web.Areas.Admin.Emulation;
using Web.Helpers.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class EmulatorController : Controller
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly Emulator _emulator;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorEmulator, SensorEmulatorListItemViewModel>()
            .ForMember(f => f.Latitude, m => m.MapFrom((s, d) => s.Latitude))
            .ForMember(f => f.Longitude, m => m.MapFrom((s, d) => s.Longitude))
            .ForMember(f => f.Guid, m => m.MapFrom(s => s.GetGuid()))
            .ForMember(f => f.IsOn, m => m.MapFrom(s => s.IsPowerOn))
            .ForMember(f => f.ApiKey, m => m.MapFrom(s => s.ApiKey))
            .ForMember(f => f.Type, m => m.MapFrom(s => s.SensorType.Name))
            ;

        }));

        public EmulatorController(ISettingsProvider settingsProvider, Emulator emulator)
        {
            _settingsProvider = settingsProvider;
            _emulator = emulator;
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<SensorEmulator> emulators = new List<SensorEmulator>();
            if (_emulator.IsEmulationEnabled)
            {
                emulators = _emulator.Devices;
            }
            return View(_mapper.Map<List<SensorEmulator>, List<SensorEmulatorListItemViewModel>>(emulators));
        }


        [HttpPost]
        public async Task<ActionResult> StartEmulation()
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return Forbid("Emulation is not available");
            }
            await _emulator.RunEmulationAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult StopEmulation()
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return Forbid("Emulation is not available");
            }
            _emulator.StopEmulation();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PowerOn(string guid)
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return Forbid("Emulation is not available");
            }
            var emulator = _emulator.Devices.FirstOrDefault(f => f.GetGuid() == guid);
            if (emulator == null)
            {
                return NotFound("Emulators not found");
            }
            if (emulator.IsPowerOn)
            {
                return Ok("Emulator has been started already");
            }

            emulator.PowerOn();
            return Ok("Emulator has been successfully started");
        }

        [HttpPost]
        public ActionResult PowerOff(string guid)
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return Forbid("Emulation is not available");
            }
            var emulator = _emulator.Devices.FirstOrDefault(f => f.GetGuid() == guid);
            if (emulator == null)
            {
                return NotFound("Emulators not found");
            }
            if (!emulator.IsPowerOn)
            {
                return Ok("Emulator has been stopped already");
            }

            emulator.PowerOff();
            return Ok("Emulator has been successfully stopped");
        }

    }
}
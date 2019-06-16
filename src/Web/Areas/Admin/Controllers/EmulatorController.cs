using System.Net;
using System.Threading.Tasks;
using Web.Data;
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
        private readonly IRepository _repository;
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly ISettingsProvider _settingsProvider;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorEmulator, SensorEmulatorListItemViewModel>()
            .ForMember(f => f.Latitude, m => m.MapFrom((s, d) => s.Latitude))
            .ForMember(f => f.Longitude, m => m.MapFrom((s, d) => s.Longitude))
            .ForMember(f => f.Guid, m => m.MapFrom(s => s.GetGuid()))
            .ForMember(f => f.IsOn, m => m.MapFrom(s => s.IsPowerOn))
            .ForMember(f => f.ApiKey, m => m.MapFrom(s => s.ApiKey));

        }));

        public EmulatorController(IRepository repository, ISensorCacheHelper sensorCacheHelper, ISettingsProvider settingsProvider)
        {
            _repository = repository;
            _sensorCacheHelper = sensorCacheHelper;
            _settingsProvider = settingsProvider;
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<SensorEmulator> emulators = new List<SensorEmulator>();
            if (Emulator.IsEmulationEnabled)
            {
                emulators = Emulator.Devices;
            }
            return View(_mapper.Map<List<SensorEmulator>, List<SensorEmulatorListItemViewModel>>(emulators));
        }


        [HttpPost]
        public async Task<ActionResult> StartEmulation()
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return NotFound("Емуляция недоступна в данной среде");
            }
            await Emulator.RunEmulationAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult StopEmulation()
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return NotFound("Емуляция недоступна в данной среде");
            }
            Emulator.StopEmulation();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PowerOn(string guid)
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return NotFound("Емуляция недоступна в данной среде");
            }
            var emulator = Emulator.Devices.Where(f => f.GetGuid() == guid).FirstOrDefault();
            if (emulator == null)
            {
                return NotFound("Емуляторы не найдены!");
            }
            if (emulator.IsPowerOn)
            {
                return Ok("Эмулятор уже запущен!");
            }
            else
            {
                emulator.PowerOn();
                return Ok("Эмулятор успешно запущен!");
            }
        }

        [HttpPost]
        public ActionResult PowerOff(string guid)
        {
            if (!_settingsProvider.EmulationEnabled)
            {
                return NotFound("Емуляция недоступна в данной среде");
            }
            var emulator = Emulator.Devices.Where(f => f.GetGuid() == guid).FirstOrDefault();
            if (emulator == null)
            {
                return NotFound("Емуляторы не найдены!");
            }
            if (!emulator.IsPowerOn)
            {
                return Ok("Эмулятор уже остановлен!");
            }
            else
            {
                emulator.PowerOff();
                return Ok("Эмулятор успешно остановлен!");
            }
        }

    }
}
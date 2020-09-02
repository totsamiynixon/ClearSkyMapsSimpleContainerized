using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Queries
{
    public class EmulationQueries : IEmulationQueries
    {
        private readonly Emulator _emulator;
        private readonly IMapper _mapper;
        private readonly EmulationAppSettings _emulationAppSettings;

        public EmulationQueries(Emulator emulator, IMapper mapper, EmulationAppSettings emulationAppSettings)
        {
            _emulator = emulator;
            _mapper = mapper;
            _emulationAppSettings = emulationAppSettings;
        }

        public Task<List<EmulatorDeviceDTO>> GetEmulatorDevicesAsync()
        {
            if (!_emulationAppSettings.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }

            return Task.FromResult(_mapper.Map<List<SensorEmulator>, List<EmulatorDeviceDTO>>(_emulator.Devices));
        }
    }
}
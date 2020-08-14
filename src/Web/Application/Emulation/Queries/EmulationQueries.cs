using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Web.Application.Emulation.Exceptions;
using Web.Application.Emulation.Queries.DTO;
using Web.Emulation;
using Web.Infrastructure;

namespace Web.Application.Emulation.Queries
{
    public class EmulationQueries : IEmulationQueries
    {
        private readonly Emulator _emulator;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public EmulationQueries(Emulator emulator, IMapper mapper, AppSettings appSettings)
        {
            _emulator = emulator;
            _mapper = mapper;
            _appSettings = appSettings;
        }

        public Task<List<EmulatorDeviceDTO>> GetEmulatorDevicesAsync()
        {
            if (!_appSettings.Emulation.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }

            return Task.FromResult(_mapper.Map<List<SensorEmulator>, List<EmulatorDeviceDTO>>(_emulator.Devices));
        }
    }
}
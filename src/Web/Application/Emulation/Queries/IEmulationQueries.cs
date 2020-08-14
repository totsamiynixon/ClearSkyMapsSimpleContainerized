using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Application.Emulation.Queries.DTO;

namespace Web.Application.Emulation.Queries
{
    public interface IEmulationQueries
    {
        Task<List<EmulatorDeviceDTO>> GetEmulatorDevicesAsync();
    }
}
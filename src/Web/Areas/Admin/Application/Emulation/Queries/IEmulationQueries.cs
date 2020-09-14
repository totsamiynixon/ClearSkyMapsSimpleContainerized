using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Areas.Admin.Application.Emulation.Queries.DTO;

namespace Web.Areas.Admin.Application.Emulation.Queries
{
    public interface IEmulationQueries
    {
        Task<List<EmulatorDeviceDTO>> GetEmulatorDevicesAsync();
    }
}
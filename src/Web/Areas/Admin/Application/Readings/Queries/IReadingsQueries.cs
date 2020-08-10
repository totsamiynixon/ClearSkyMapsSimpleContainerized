using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Areas.Admin.Application.Readings.Queries.DTO;

namespace Web.Areas.Admin.Application.Readings.Queries
{
    public interface IReadingsQueries
    {
        Task<List<SensorDTO>> GetSensorsAsync();

        Task<SensorDTO> GetSensorByIdAsync(int id);
    }
}
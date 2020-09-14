using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Application.Readings.Queries.DTO;

namespace Web.Application.Readings.Queries
{
    public interface IReadingsQueries
    {
        Task<List<StaticSensorDTO>> GetStaticSensorsAsync();
    }
}
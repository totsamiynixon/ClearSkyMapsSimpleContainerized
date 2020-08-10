using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Areas.PWA.Application.Readings.Queries.DTO;

namespace Web.Areas.PWA.Application.Readings.Queries
{
    public interface IReadingsQueries
    {
        Task<List<StaticSensorDTO>> GetStaticSensorsAsync();
    }
}
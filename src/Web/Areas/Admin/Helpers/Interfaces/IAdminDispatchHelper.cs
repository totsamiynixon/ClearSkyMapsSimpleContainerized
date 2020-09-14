using System.Threading.Tasks;
using Web.Domain.Entities;

namespace Web.Areas.Admin.Helpers.Interfaces
{
    public interface IAdminDispatchHelper
    {
        Task DispatchReadingsForStaticSensorAsync(int sensorId, Reading reading);

        Task DispatchReadingsForPortableSensorAsync(int sensorId, Reading reading);

        Task DispatchCoordinatesForPortableSensorAsync(int sensorId, double latitude, double longitude);
    }
}

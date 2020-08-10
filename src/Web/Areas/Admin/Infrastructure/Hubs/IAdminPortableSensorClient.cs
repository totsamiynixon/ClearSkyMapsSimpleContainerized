using System.Threading.Tasks;
using Web.Areas.Admin.Models.Default.Hub;

namespace Web.Areas.Admin.Infrastructure.Hubs
{
    public interface IAdminPortableSensorClient
    {
        //[HubMethodName("DispatchReading")]
        Task DispatchReading(PortableSensorReadingsDispatchModel reading);
        //[HubMethodName("DispatchCoordinates")]
        Task DispatchCoordinatesAsync(PortableSensorCoordinatesDispatchModel coordinates);
    }
}

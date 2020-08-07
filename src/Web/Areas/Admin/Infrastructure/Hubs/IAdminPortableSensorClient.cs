using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Web.Areas.Admin.Models.Hub;

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

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web.Areas.Admin.Infrastructure.Hubs
{
    public class AdminPortableSensorHub : Hub<IAdminPortableSensorClient>
    {
        public static string PortableSensorGroup(int sensorId) => $"PortableSensorGroup_{sensorId}";

        public async Task ListenForSensor(int sensorId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, PortableSensorGroup(sensorId));
        }
    }
}

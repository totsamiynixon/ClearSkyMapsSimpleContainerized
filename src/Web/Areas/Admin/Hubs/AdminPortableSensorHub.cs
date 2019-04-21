using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Admin.Hubs
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

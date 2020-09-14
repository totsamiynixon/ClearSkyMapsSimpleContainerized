using Microsoft.AspNetCore.SignalR;

namespace Web.Areas.PWA.Hubs
{
    public class PWAStaticSensorHub : Hub<IPWAStaticSensorClient>
    {
    }
}

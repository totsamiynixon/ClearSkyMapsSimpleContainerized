using Microsoft.AspNetCore.SignalR;

namespace Web.Areas.Admin.Infrastructure.Hubs
{
    public class AdminStaticSensorHub : Hub<IAdminStaticSensorClient>
    {
    }
}

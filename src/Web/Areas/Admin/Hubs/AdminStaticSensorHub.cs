using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Admin.Hubs
{
    public class AdminStaticSensorHub : Hub<IAdminStaticSensorClient>
    {
    }
}

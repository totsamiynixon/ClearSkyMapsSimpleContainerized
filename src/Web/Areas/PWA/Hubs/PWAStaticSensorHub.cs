using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.PWA.Hubs
{
    public class PWAStaticSensorHub : Hub<IPWAStaticSensorClient>
    {
    }
}

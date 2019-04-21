using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Hub;

namespace Web.Areas.PWA.Hubs
{
    public interface IPWAStaticSensorClient
    {
        Task DispatchReading(StaticSensorReadingDispatchModel reading);
    }
}

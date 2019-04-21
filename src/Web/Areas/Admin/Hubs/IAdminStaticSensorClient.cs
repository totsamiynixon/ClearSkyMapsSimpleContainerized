using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Hub;

namespace Web.Areas.Admin.Hubs
{
    public interface IAdminStaticSensorClient
    {
        Task DispatchReading(StaticSensorReadingDispatchModel reading);
    }
}

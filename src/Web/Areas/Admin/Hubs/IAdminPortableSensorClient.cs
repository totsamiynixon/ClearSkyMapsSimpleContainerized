using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.Admin.Models.Hub;

namespace Web.Areas.Admin.Hubs
{
    public interface IAdminPortableSensorClient
    {
        Task DispatchReading(PortableSensorReadingsDispatchModel reading);
        Task DispatchCoordinates(PortableSensorCoordinatesDispatchModel coordinates);
    }
}

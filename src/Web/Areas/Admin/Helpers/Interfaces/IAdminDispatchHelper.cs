using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;

namespace Web.Areas.Admin.Helpers.Interfaces
{
    public interface IAdminDispatchHelper
    {
        void DispatchReadingsForStaticSensor(int sensorId, Reading reading);

        void DispatchReadingsForPortableSensor(int sensorId, Reading reading);

        void DispatchCoordinatesForPortableSensor(int sensorId, double latitude, double longitude);
    }
}

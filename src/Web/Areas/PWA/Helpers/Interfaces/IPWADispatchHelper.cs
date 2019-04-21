using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Enums;

namespace Web.Areas.PWA.Helpers.Interfaces
{
    public interface IPWADispatchHelper
    {
        void DispatchReadingsForStaticSensor(int sensorId, PollutionLevel pollutionLevel, Reading reading);
    }
}

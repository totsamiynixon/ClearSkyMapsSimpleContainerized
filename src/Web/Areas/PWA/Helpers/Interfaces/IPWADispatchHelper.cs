using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.Areas.PWA.Helpers.Interfaces
{
    public interface IPWADispatchHelper
    {
        void DispatchReadingsForStaticSensor(int sensorId, PollutionLevel pollutionLevel, Reading reading);
    }
}

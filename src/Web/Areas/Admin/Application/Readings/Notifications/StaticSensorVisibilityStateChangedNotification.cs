using MediatR;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class StaticSensorVisibilityStateChangedNotification : INotification
    {
        public StaticSensorVisibilityStateChangedNotification(int sensorId)
        {
            SensorId = sensorId;
        }

        public int SensorId { get; }
        
    }
}
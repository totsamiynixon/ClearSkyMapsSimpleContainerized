using MediatR;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class SensorDeletedNotification : INotification
    {
        public SensorDeletedNotification(int sensorId)
        {
            SensorId = sensorId;
        }

        public int SensorId { get; }
    }
}
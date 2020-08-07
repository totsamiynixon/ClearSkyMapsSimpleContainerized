using MediatR;
using Web.Application.Readings.DTO;

namespace Web.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotification : INotification
    {
        public SensorReadingDTO Reading { get; }
        
        public int SensorId { get; }
        
        public StaticSensorReadingCreatedNotification(int sensorId, SensorReadingDTO reading)
        {
            Reading = reading;
            SensorId = sensorId;
        }
    }
}
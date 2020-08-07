using MediatR;
using Web.Application.Readings.DTO;

namespace Web.Application.Readings.Notifications
{
    public class PortableReadingCreatedNotification : INotification
    {
        public SensorReadingDTO Reading { get; }
        
        public int SensorId { get; }
        
        public PortableReadingCreatedNotification(int sensorId, SensorReadingDTO reading)
        {
            Reading = reading;
            SensorId = sensorId;
        }
    }
}
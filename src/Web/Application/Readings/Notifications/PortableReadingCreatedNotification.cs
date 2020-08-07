using MediatR;
using Web.Application.Readings.DTO;
using Web.Domain.Entities;

namespace Web.Application.Readings.Notifications
{
    public class PortableReadingCreatedNotification : INotification
    {
        public PortableSensorReading Reading { get; }
        
        public int SensorId { get; }
        
        public PortableReadingCreatedNotification(int sensorId, PortableSensorReading reading)
        {
            Reading = reading;
            SensorId = sensorId;
        }
    }
}
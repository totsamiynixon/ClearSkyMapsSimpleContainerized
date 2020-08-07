using MediatR;
using Web.Application.Readings.DTO;
using Web.Domain.Entities;

namespace Web.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotification : INotification
    {
        public StaticSensorReading Reading { get; }
        
        public int SensorId { get; }
        
        public StaticSensorReadingCreatedNotification(int sensorId, StaticSensorReading reading)
        {
            Reading = reading;
            SensorId = sensorId;
        }
    }
}
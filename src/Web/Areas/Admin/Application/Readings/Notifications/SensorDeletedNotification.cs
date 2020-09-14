using MediatR;
using Web.Domain.Entities;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class SensorDeletedNotification : INotification
    {
        public SensorDeletedNotification(Sensor deletedSensor)
        {
            DeletedSensor = deletedSensor;
        }

        public Sensor DeletedSensor { get; }
    }
}
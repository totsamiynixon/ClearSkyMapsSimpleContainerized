using MediatR;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class SensorActivationStateChangedNotification : INotification
    {
        public int SensorId { get;}
        
        public SensorActivationStateChangedNotification(int sensorId)
        {
            SensorId = sensorId;
        }
    }
}
using MediatR;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class ChangeStaticSensorVisibilityStateCommand : IRequest<bool>
    {
        public ChangeStaticSensorVisibilityStateCommand(int sensorId, bool isVisible)
        {
            SensorId = sensorId;
            IsVisible = isVisible;
        }

        public int SensorId { get; }
        
        public bool IsVisible { get; }
    }
}
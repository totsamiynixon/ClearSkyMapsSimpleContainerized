using MediatR;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class ChangeSensorActivationStateCommand : IRequest<bool>
    {
        public ChangeSensorActivationStateCommand(int sensorId, bool isActive)
        {
            SensorId = sensorId;
            IsActive = isActive;
        }

        public int SensorId { get; }
        
        public bool IsActive { get; }
    }
}
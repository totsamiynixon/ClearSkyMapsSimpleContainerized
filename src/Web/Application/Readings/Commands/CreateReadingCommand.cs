using MediatR;
using Web.Application.Readings.DTO;

namespace Web.Application.Readings.Commands
{
    public class CreateReadingCommand : IRequest<bool>
    {
        public SensorReadingDTO Reading { get; }
        
        public string ApiKey { get; }
        
        public CreateReadingCommand(SensorReadingDTO reading, string apiKey)
        {
            Reading = reading;
            ApiKey = apiKey;
        }
    }
}
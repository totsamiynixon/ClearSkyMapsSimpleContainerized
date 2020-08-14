using MediatR;
using Web.Application.Readings.Commands.DTO;

namespace Web.Application.Readings.Commands
{
    public class CreateReadingCommand : IRequest<bool>
    {
        public StaticSensorReadingDTO Reading { get; }
        
        public string ApiKey { get; }
        
        public CreateReadingCommand(StaticSensorReadingDTO reading, string apiKey)
        {
            Reading = reading;
            ApiKey = apiKey;
        }
    }
}
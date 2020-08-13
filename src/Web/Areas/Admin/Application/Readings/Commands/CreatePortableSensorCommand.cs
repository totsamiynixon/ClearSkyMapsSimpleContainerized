using MediatR;
using Web.Areas.Admin.Application.Readings.DTO;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreatePortableSensorCommand : IRequest<PortableSensorDTO>
    {
        public CreatePortableSensorCommand(string apiKey)
        {
            ApiKey = apiKey;
        }
        public string ApiKey { get; }
    }
}
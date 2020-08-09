using MediatR;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreatePortableSensorCommand : IRequest<bool>
    {
        public CreatePortableSensorCommand(string apiKey)
        {
            ApiKey = apiKey;
        }
        public string ApiKey { get; }
    }
}
using MediatR;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class DeleteSensorCommand : IRequest<bool>
    {
        public DeleteSensorCommand(int id, bool isCompletely)
        {
            Id = id;
            IsCompletely = isCompletely;
        }
        
        public int Id { get; }
        public bool IsCompletely { get;  }
    }
}
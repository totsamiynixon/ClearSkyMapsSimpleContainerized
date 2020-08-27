using MediatR;
using Web.Areas.Admin.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Notifications
{
    public class EmulationStoppedNotification : INotification
    {
        public Emulator Emulator { get; }
        
        public EmulationStoppedNotification(Emulator emulator)
        {
            Emulator = emulator;
        }
    }
}
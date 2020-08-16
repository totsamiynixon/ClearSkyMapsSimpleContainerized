using MediatR;
using Web.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Notifications
{
    public class EmulationStartedNotification : INotification
    {
        public Emulator Emulator { get; }

        public EmulationStartedNotification(Emulator emulator)
        {
            Emulator = emulator;
        }
    }
}
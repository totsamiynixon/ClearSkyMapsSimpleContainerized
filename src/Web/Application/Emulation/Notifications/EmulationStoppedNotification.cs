using MediatR;
using Web.Emulation;

namespace Web.Application.Emulation.Notifications
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
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ChangeActivationSensorViewModel
    {
        public ChangeActivationSensorViewModel(ChangeActivationSensorModel model, SensorDetailsViewModel details)
        {
            Model = model;
            Details = details;
        }

        public ChangeActivationSensorModel Model { get; }
        public SensorDetailsViewModel Details { get; }
    }
}
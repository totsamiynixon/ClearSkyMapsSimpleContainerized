namespace Web.Areas.Admin.Models.Default.Sensors
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
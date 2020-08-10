namespace Web.Areas.Admin.Models.Sensors
{
    public class ChangeVisibilityStaticSensorViewModel
    {
        public ChangeVisibilityStaticSensorViewModel(ChangeVisibilityStaticSensorModel model, SensorDetailsViewModel details)
        {
            Model = model;
            Details = details;
        }

        public ChangeVisibilityStaticSensorModel Model { get; }
        public SensorDetailsViewModel Details { get; }
    }
}
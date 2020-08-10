namespace Web.Areas.Admin.Models.Sensors
{
    public class CreateStaticSensorViewModel
    {
        public CreateStaticSensorViewModel(CreateStaticSensorModel model)
        {
            Model = model;
        }

        public CreateStaticSensorModel Model { get; }
    }
}
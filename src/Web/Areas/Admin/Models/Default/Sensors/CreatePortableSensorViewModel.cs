namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class CreatePortableSensorViewModel
    {
        public CreatePortableSensorViewModel(CreatePortableSensorModel model)
        {
            Model = model;
        }

        public CreatePortableSensorModel Model { get; }
    }
}
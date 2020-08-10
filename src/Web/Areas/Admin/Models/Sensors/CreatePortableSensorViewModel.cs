namespace Web.Areas.Admin.Models.Sensors
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
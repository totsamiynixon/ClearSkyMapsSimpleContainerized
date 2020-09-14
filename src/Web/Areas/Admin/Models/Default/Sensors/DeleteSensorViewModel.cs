﻿namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class DeleteSensorViewModel
    {
        public DeleteSensorViewModel(DeleteSensorModel model, SensorDetailsViewModel details)
        {
            Model = model;
            Details = details;
        }

        public DeleteSensorModel Model { get; }

        public SensorDetailsViewModel Details { get; }
    }
}
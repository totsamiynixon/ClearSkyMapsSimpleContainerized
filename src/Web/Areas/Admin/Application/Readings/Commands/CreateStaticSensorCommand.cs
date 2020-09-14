﻿using MediatR;
using Web.Areas.Admin.Application.Readings.Commands.DTO;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreateStaticSensorCommand : IRequest<StaticSensorDTO>
    {
        public CreateStaticSensorCommand(string apiKey, double latitude, double longitude)
        {
            ApiKey = apiKey;
            Latitude = latitude;
            Longitude = longitude;
        }
        
        public string ApiKey { get; }
        
        public double Latitude { get; }
        
        public double Longitude { get; }
    }
}
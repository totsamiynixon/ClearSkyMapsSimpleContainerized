using System.Collections.Generic;
using Web.Domain.Entities;
using Web.Helpers;

namespace Web.IntegrationTests.Infrastructure
{
    public class Defaults
    {
        public const double Latitude = 53.333333;
        public const double Longitude = 51.111111;

        public static StaticSensor StaticSensor => new StaticSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            Latitude = Latitude,
            Longitude = Longitude,
            IsActive = false,
            IsVisible = false,
            Readings = new List<StaticSensorReading>()
        };
        
        public static StaticSensor ActiveStaticSensor => new StaticSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            Latitude = Latitude,
            Longitude = Longitude,
            IsActive = true,
            IsVisible = false,
            Readings = new List<StaticSensorReading>()
        };
        
        public static StaticSensor ActiveAndVisibleStaticSensor => new StaticSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            Latitude = Latitude,
            Longitude = Longitude,
            IsActive = true,
            IsVisible = true,
            Readings = new List<StaticSensorReading>()
        };
        
        public static StaticSensor DeletedStaticSensor => new StaticSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            Latitude = Latitude,
            Longitude = Longitude,
            IsActive = false,
            IsVisible = false,
            IsDeleted = true,
            Readings = new List<StaticSensorReading>()
        };

        public static PortableSensor PortableSensor => new PortableSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            IsActive = false
        };
        
        public static PortableSensor ActivePortableSensor => new PortableSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            IsActive = true
        };
        
        public static PortableSensor DeletedPortableSensor => new PortableSensor
        {
            ApiKey = CryptoHelper.GenerateApiKey(),
            IsActive = false,
            IsDeleted = true
        };
    }
}
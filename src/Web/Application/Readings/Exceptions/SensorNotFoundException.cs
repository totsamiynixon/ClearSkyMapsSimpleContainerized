using System;

namespace Web.Application.Readings.Exceptions
{
    public class SensorNotFoundException : Exception
    {
        public SensorNotFoundException(int sensorId) : base($"Sensor with id: {sensorId} was not found!")
        {
            
        }
        
        public SensorNotFoundException(string apiKey) : base($"Sensor with api key: {apiKey} was not found!")
        {
            
        }
    }
}
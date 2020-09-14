using System;
using System.Globalization;

namespace Web.Areas.Admin.Emulation.Models
{
    public class SensorDataModel
    {
        public string ApiKey { get; set; }

        public float CO2 { get; set; }

        public float LPG { get; set; }

        public float CO { get; set; }

        public float CH4 { get; set; }

        public float Dust { get; set; }

        public float Temp { get; set; }

        public float Hum { get; set; }

        public float Preassure { get; set; }

        public DateTime Created { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString()
        {
            return string.Join(",", ApiKey, Temp.ToString(CultureInfo.InvariantCulture),
                Hum.ToString(CultureInfo.InvariantCulture), Preassure.ToString(CultureInfo.InvariantCulture),
                CO2.ToString(CultureInfo.InvariantCulture), LPG.ToString(CultureInfo.InvariantCulture),
                CO.ToString(CultureInfo.InvariantCulture), CH4.ToString(CultureInfo.InvariantCulture),
                Dust.ToString(CultureInfo.InvariantCulture), Longitude.ToString(CultureInfo.InvariantCulture),
                Latitude.ToString(CultureInfo.InvariantCulture));
        }
    }
}
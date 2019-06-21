using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Device.Location;

namespace Monitor
{
    class Program
    {
        private const string URL = "";

        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Set_GPS();
            var serialPort1 = new SerialPort();
            serialPort1.PortName = "COM3"; //Указываем наш порт - в данном случае COM1.
            serialPort1.BaudRate = 9600; //указываем скорость.
            serialPort1.DataBits = 8;
            serialPort1.Open();

            try
            {
                while (true)
                {
                    var data = serialPort1.ReadLine();
                    if (data.Length < 20)
                    {
                        continue;
                    }
                    Console.WriteLine(data);
                    var model = GetModelFromString(data);
                    MakePostRequest(model);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                serialPort1.Close();
            }
        }

        private static void MakePostRequest(SensorDataModel model)
        {
            var values = JsonConvert.SerializeObject(model);
            using (var wb = new WebClient())
            {
                try
                {
                    wb.Headers.Add("Content-Type", "application/json");
                    var response = wb.UploadString(URL, "POST", values);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

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
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public DateTime Created { get; set; }
        }

        private static SensorDataModel GetModelFromString(string data)
        {
            try
            {
                var deserialized = JsonConvert.DeserializeObject<JObject>(data);
                var serializedData = deserialized["data"].ToString();
                var groupes = serializedData.Replace("; ", ";").Replace('.', ',').Split(';').ToArray();
                return new SensorDataModel
                {
                    ApiKey = groupes[0],
                    Temp = float.Parse(groupes[1]),
                    Hum = float.Parse(groupes[2]),
                    Preassure = float.Parse(groupes[3]),
                    CO2 = float.Parse(groupes[4]),
                    LPG = float.Parse(groupes[5]),
                    CO = float.Parse(groupes[6]),
                    CH4 = float.Parse(groupes[7]),
                    Dust = float.Parse(groupes[8]),
                    Longitude = Longitude,
                    Latitude = Latitude,
                    Created = DateTime.Now

                };
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        #region Geolocation

        private static double Latitude { get; set; }

        private static double Longitude { get; set; }

        private static void Set_GPS()
        {
            GeoCoordinateWatcher watcher;

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
            {
                MovementThreshold = 20
            };

            watcher.PositionChanged += (o, e) =>
            {

                var epl = e.Position.Location;

                // Access the position information thusly:
                Latitude = epl.Latitude;
                Longitude = epl.Longitude;
                epl.Altitude.ToString();
                epl.HorizontalAccuracy.ToString();
                epl.VerticalAccuracy.ToString();
                epl.Course.ToString();
                epl.Speed.ToString();
                e.Position.Timestamp.LocalDateTime.ToString();
            };
            watcher.Start();
        }
        #endregion
    }
}

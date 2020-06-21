using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;

namespace Web.Data.Models
{
    public abstract class Reading
    {

        public Reading()
        {
            Created = DateTime.UtcNow;
        }
        public int Id { get; set; }
        [PDK(300, 4)]
        public float CO2 { get; set; }
        [PDK(300, 4)]
        public float LPG { get; set; }
        [PDK(3.0, 3)]
        public float CO { get; set; }
        [PDK(0.716, 2)]
        public float CH4 { get; set; }
        public float Dust { get; set; }
        public float Temp { get; set; }
        public float Hum { get; set; }
        public float Preassure { get; set; }
        public DateTime Created { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Api.Notifications
{
    public class SubscribeOnSersorModel
    {
        public string RegistrationToken { get; set; }

        public int SensorId { get; set; }
    }
}
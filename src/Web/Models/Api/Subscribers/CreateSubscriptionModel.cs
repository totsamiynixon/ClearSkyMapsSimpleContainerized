using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Api.Subscribers
{
    public class CreateSubscriptionModel
    {
        public List<int> SensorIds { get; set; }
    }
}
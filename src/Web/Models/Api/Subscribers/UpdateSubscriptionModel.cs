using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Api.Subscribers
{
    public class UpdateSubscriptionModel
    {
        public string Id { get; set; }

        public List<int> SensorIds { get; set; }
    }
}
using System.Collections.Generic;

namespace Web.Areas.PWA.Models.Api.Subscribers
{
    public class CreateSubscriptionModel
    {
        public List<int> SensorIds { get; set; }
    }
}
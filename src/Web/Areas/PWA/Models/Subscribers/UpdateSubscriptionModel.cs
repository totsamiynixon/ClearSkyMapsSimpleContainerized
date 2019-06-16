using System.Collections.Generic;

namespace Web.Areas.PWA.Models.Api.Subscribers
{
    public class UpdateSubscriptionModel
    {
        public string Id { get; set; }

        public List<int> SensorIds { get; set; }
    }
}
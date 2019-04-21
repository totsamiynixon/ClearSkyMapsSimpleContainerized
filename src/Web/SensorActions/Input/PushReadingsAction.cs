using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.SensorActions.Input
{
    public class PushReadingsAction : InputSensorAction<PushReadingsActionPayload>
    {
        public PushReadingsAction(PushReadingsActionPayload payload) : base(InputSensorActionType.PushReadings, payload)
        {
        }
    }
}
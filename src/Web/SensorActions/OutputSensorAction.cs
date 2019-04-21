using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.SensorActions
{
    public abstract class OutputSensorAction<T> : SensorAction<OuputSensorActionType, T> where T : class
    {
        public OutputSensorAction(OuputSensorActionType type, T payload) : base(type, payload)
        {
        }
    }
}
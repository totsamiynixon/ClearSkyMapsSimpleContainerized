using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.SensorActions
{
    public abstract class InputSensorAction<T> : SensorAction<InputSensorActionType, T> where T : class
    {
        public InputSensorAction(InputSensorActionType type, T payload) : base(type, payload)
        {
        }
    }
}
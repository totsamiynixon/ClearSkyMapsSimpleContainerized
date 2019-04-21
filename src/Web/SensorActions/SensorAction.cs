using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.SensorActions
{
    public abstract class SensorAction<E, T> where T : class
    {
        public SensorAction(E type, T payload)
        {
            Type = type;
            Payload = payload;
        }

        public E Type { get; set; }

        public T Payload { get; set; }
    }
}
using System;

namespace Web.Areas.Admin.Application.Readings.Exceptions
{
    public class SensorUnableApplyActionException : Exception
    {
        public Actions Action { get; private set; }
        public SensorUnableApplyActionException(Actions action, string reason) : base(
            $"Unable to apply {action}, reason: {reason}")
        {
            Action = action;
        }

        public enum Actions
        {
            Delete
        }
    }
}
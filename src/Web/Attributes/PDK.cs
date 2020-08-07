using System;

namespace Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PDK : Attribute
    {
        public double PDKValue { get; set; }
        public int LevelOfDanger { get; set; }
        public PDK(double pdk, int levelOfDanger)
        {
            PDKValue = pdk;
            LevelOfDanger = levelOfDanger;
        }
    }
}

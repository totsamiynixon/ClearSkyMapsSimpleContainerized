using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Enums;
using Web.Helpers.Interfaces;

namespace Web.Helpers.Implementations
{
    public class PollutionCalculator : IPollutionCalculator
    {
        private static Random _random = new Random();

        public PollutionLevel CalculatePollutionLevel(List<Reading> readings)
        {
            if (!readings.Any())
            {
                return PollutionLevel.Unknown;
            }
            return (PollutionLevel)_random.Next(0, 2);
        }
    }
}

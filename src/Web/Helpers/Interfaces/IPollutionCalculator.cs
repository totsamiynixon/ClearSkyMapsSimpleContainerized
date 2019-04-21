using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Enums;

namespace Web.Helpers.Interfaces
{
    public interface IPollutionCalculator
    {
        PollutionLevel CalculatePollutionLevel(List<Reading> readings);
    }
}

using System.Collections.Generic;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.Helpers.Interfaces
{
    public interface IPollutionCalculator
    {
        PollutionLevel CalculatePollutionLevel(List<Reading> readings);
    }
}

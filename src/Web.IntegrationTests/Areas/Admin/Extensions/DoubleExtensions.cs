using System.Globalization;

namespace Web.IntegrationTests.Areas.Admin.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToCommaSeparatedString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
        }
    }
}
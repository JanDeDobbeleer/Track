using System.Linq;

namespace TrackApi.Tools
{
    public static class Extensions
    {
        public static string AmericanDateToApiDate(this string date)
        {
            if (!date.Contains("-"))
                return date;
            var array = date.Split('-');
            return array[1] + array[0] + array[2];
        }
    }
}

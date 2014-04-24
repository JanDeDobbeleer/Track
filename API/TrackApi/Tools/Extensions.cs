using System.Linq;

namespace TrackApi.Tools
{
    public static class Extensions
    {
        public static string ConvertDateToCorrectDateStamp(this string date)
        {
            if (!date.Contains("-"))
                return date;
            var array = date.Split('-');
            return array.Aggregate(string.Empty, (current, s) => current + s.AddZero());
        }

        public static string AddZero(this string temp)
        {
            if (temp.Length == 1)
                return "0" + temp;
            return temp;
        }

        public static string AmericanDateToApiDate(this string date)
        {
            if (!date.Contains("-"))
                return date;
            var array = date.Split('-');
            return array[1] + array[0] + array[2];
        }
    }
}

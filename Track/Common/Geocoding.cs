using System;
using System.Device.Location;

namespace Track.Common
{
    public enum DistanceMeasure
    {
        Miles,
        Kilometers
    }

    public static class Geocoding
    {
        //const Double EarthRadiusInMiles = 3956.0;
        const Double EarthRadiusInKm = 6366.707019;

        //helper method to make reading the lambda a bit easier
        static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        //helper method for converting Radians, making the lamda easier to read
        static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        public static double CalculateDistanceFrom(GeoCoordinate from, GeoCoordinate to)
        {
            return CalculateDistanceFrom(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
        }

        public static double CalculateDistanceFrom(double fromLat, double fromLon, double toLat, double toLong)
        {
            Func<double, double, double, double, double> CalcDistance = (lat1, lon1, lat2, lon2) =>
            EarthRadiusInKm * 2 *
            (
                Math.Asin(
                    Math.Min(1,
                        Math.Sqrt(
                            (
                                Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) +
                                Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) *
                                Math.Pow(Math.Sin((DiffRadian(lon1, lon2)) / 2.0), 2.0)
                            )
                       )
                   )
               )
             )
            ;

            return CalcDistance(fromLat, fromLon, toLat, toLong);
        }

        /*Position, decimal degrees
         lat = 51.0
         lon = 0.0

         //Earth’s radius, sphere
         R=6378137

         //offsets in meters
         dn = 100
         de = 100

         //Coordinate offsets in radians
         dLat = dn/R
         dLon = de/(R*Cos(Pi*lat/180))

         //OffsetPosition, decimal degrees
         latO = lat + dLat * 180/Pi
         lonO = lon + dLon * 180/Pi */

        public static GeoCoordinate OffsetCoordinate(this GeoCoordinate cord, int offset)
        {
            var temp = new GeoCoordinate
            {
                Longitude = cord.Longitude + (offset/(6378137*Math.Cos(Math.PI*(cord.Longitude/180))))*180/Math.PI,
                Latitude = cord.Latitude + (offset/(double) 6378137)*180/Math.PI
            };
            return temp;
        }
    }
}

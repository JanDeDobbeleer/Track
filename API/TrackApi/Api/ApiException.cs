using System;

namespace TrackApi.Api
{
    public class ApiException:Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }
}

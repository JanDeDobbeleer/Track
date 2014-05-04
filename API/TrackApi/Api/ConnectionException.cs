using System;

namespace TrackApi.Api
{
    public class ConnectionException:Exception
    {
        public ConnectionException(string message) : base(message)
        {
            
        }
    }
}

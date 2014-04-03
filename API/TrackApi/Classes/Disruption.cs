using System;

namespace TrackApi.Classes
{
    /*<item>
        <title>Lijn 130 : Charleroi-Zuid - Namen Vertraagd verkeer. </title>
        <link>
        http://www.railtime.be/website/Pages/InfobarList.aspx?l=NL
        </link>
        <description>06:50 (26/03/2014) Er is een storing aan de seininrichting tussen station Dinant en station Yvoir. Hierdoor duurt de reistijd 10 tot 15 minuten langer.   </description>
        <pubDate>Wed, 26 Mar 2014 05:54:00 GMT</pubDate>
      </item>
    */
    public class Disruption
    {
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public string Link { get; set; }
    }
}

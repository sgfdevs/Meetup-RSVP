using System.Collections.Generic;
using Meetup.NetStandard.Response.Events;
using Meetup.NetStandard.Response.Rsvps;

namespace SGFDevsMeetup.Data
{
    public class DevsMeetupList
    {
        public List<Event> Events { get; set; }
        public List<Rsvp> Rsvps { get; set; }
        public List<Rsvp> FullRsvps { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.NetStandard;
using Meetup.NetStandard.Request.Events;
using Meetup.NetStandard.Request.Rsvps;
using Meetup.NetStandard.Response.Events;
using Meetup.NetStandard.Response.Rsvps;

namespace SGFDevsMeetup.Data
{
    public class MeetupService
    {
        public async Task<DevsMeetupList> GetMeetings()
        {
            var meetup = new MeetupClient(null);
            var today = DateTime.Now;
            
            string[] sgfDevGroups = {"sgfdevs", "sgfdotnet", "sgf-aws"};
            var eventList = new List<Event>();
            var rsvpList = new List<Rsvp>();
            var fullRsvpList = new List<Rsvp>();

            foreach (var devGroup in sgfDevGroups)
            {
                var meetingListRequest = new GetEventsRequest(devGroup);

                var meetingList = await meetup.Events.For(meetingListRequest);

                eventList.AddRange(meetingList.Data.Where(meeting => 
                    meeting.Time.DayOfWeek == DayOfWeek.Wednesday 
                    && meeting.Time.Month == today.Month 
                    && meeting.Time.Day <= 7));
            }

            foreach (var meeting in eventList)
            {
                var request = new GetRsvpsRequest(meeting.Group.UrlName, meeting.Id)
                {
                    Response = RsvpStatus.YesOnly,
                    OrderBy = RsvpOrderBy.Time,
                    Descending = true
                };

                var rsvps = await meetup.Rsvps.For(request);
                fullRsvpList.AddRange(rsvps.Data);
            }
            
            
            rsvpList = fullRsvpList.ToLookup(m => m.Member.Id).Select(col => col.First()).ToList();

            return new DevsMeetupList
            {
                Events = eventList,
                Rsvps = rsvpList,
                FullRsvps = fullRsvpList
            };
        }
    }
}
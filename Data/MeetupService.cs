using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Meetup.NetStandard;
using Meetup.NetStandard.Request.Events;
using Meetup.NetStandard.Request.Rsvps;
using Meetup.NetStandard.Response.Events;
using Meetup.NetStandard.Response.Rsvps;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace SGFDevsMeetup.Data
{

    public class DevsEvent : Event
    {
        [JsonProperty("rsvp_close_offset")]
        public string RsvpCloseOffset { get; set; }
    }
    
    public class MeetupService
    {
        private readonly HttpClient http;

        public MeetupService(HttpClient httpInstance)
        {
            http = httpInstance;
        }
        public async Task<DevsMeetupList> GetMeetings()
        {
            var today = DateTime.Now;

            var timeZoneOffset = new TimeSpan(6,0,0);
            
            string[] sgfDevGroups = {"sgfdevs", "sgfdotnet", "sgf-aws"};
            var daysOfTheWeekPostMeetup = new[] {DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday};
            var corsProxy = "https://cors-anywhere.herokuapp.com/";
            var eventList = new List<DevsEvent>();
            var rsvpList = new List<Rsvp>();
            var fullRsvpList = new List<Rsvp>();
            var monthToCheck = today.Month;
            
            if (today.Day > 7 || daysOfTheWeekPostMeetup.Contains(today.DayOfWeek))
            {
                monthToCheck = today.Month + 1;
            }

            foreach (var devGroup in sgfDevGroups)
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = new HttpMethod("GET"),
                    RequestUri = new Uri(corsProxy + "https://api.meetup.com/"+ devGroup + "/events?&sign=true&photo-host=public&page=20")
                };
                
                var response = await http.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var meetingList = JsonConvert.DeserializeObject<List<DevsEvent>>(responseContent).Select(m =>
                {
                    m.Time = m.Time - timeZoneOffset;
                    return m;
                }).ToList();

                eventList.AddRange(meetingList.Where(meeting => 
                    meeting.RsvpCloseOffset == null
                    && meeting.Time.DayOfWeek == DayOfWeek.Wednesday 
                    && meeting.Time.Month == monthToCheck
                    && meeting.Time.Day <= 7));
            }

            foreach (var meeting in eventList)
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Method = new HttpMethod("GET"),
                    RequestUri = new Uri(corsProxy + "https://api.meetup.com/"+ meeting.Group.UrlName +"/events/"+ meeting.Id +"/rsvps?&sign=true&photo-host=public&response=yes&order=time&desc=true")
                };
                
                var response = await http.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var rsvps = JsonConvert.DeserializeObject<List<Rsvp>>(responseContent);
                
                fullRsvpList.AddRange(rsvps);
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
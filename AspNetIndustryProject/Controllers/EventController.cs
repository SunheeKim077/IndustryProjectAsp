using AspNetIndustryProject.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Microsoft.AspNetCore.Hosting;

namespace AspNetIndustryProject.Controllers
{
    public class CalendarEvent
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
    public class EventController : Controller
    {     

        public List<CalendarEvent> GoogleEvents = new List<CalendarEvent>();

        /* Global instance of the scopes required by this quickstart.
         If modifying these scopes, delete your previously saved token.json/ folder. */
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public IActionResult Index()
        {
            CalendarEvents();
            ViewBag.EventList = GoogleEvents;
            return View();
        }

        public void CalendarEvents()
        {
            UserCredential credential;

            // Load client secrets.
            // Get credentials from secrets.json
            using (var stream =
                   new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\skim0\\AppData\\Roaming\\Microsoft\\UserSecrets\\df643faf-b676-48ad-80d6-0ed14d6c87c0\\secrets.json"), FileMode.Open, FileAccess.Read)) // UserSecretsId.
            {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("31cf394c0237504f1a86c0cdd52fe6faf877af0cd758d7c0d02354132653b4fd@group.calendar.google.com");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            //Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    var calendarEvent = new CalendarEvent();
                    calendarEvent.Summary = eventItem.Summary;
                    calendarEvent.Description = eventItem.Description;
                    calendarEvent.Start = eventItem.Start.DateTime.ToString();
                    calendarEvent.End = eventItem.End.DateTime.ToString();
                    GoogleEvents.Add(calendarEvent);
                }
            }
        }
    }
        
}

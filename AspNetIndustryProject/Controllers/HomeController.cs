using AspNetIndustryProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AspNetIndustryProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OauthRedirect()
        {
            var credentialsFile = "C:\\Users\\skim0\\OneDrive - Manukau Institute of Technology\\Documents\\Level 7(2023)\\AspNetIndustryProject\\AspNetIndustryProject\\Files\\credentials.json";

            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            var client_id = credentials["client_id"];


            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                            "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events&" +
                            "access_type=offline&" +
                            "include_granted_scopes=true&" +
                            "response_type=code&" +
                            "state=HelloWorld&" +
                            "redirect_uri=https://localhost:7099/oauth/callback&" +
                            "client_id=" + client_id;

            return Redirect(redirectUrl);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AspNetIndustryProject.Controllers
{
    public class OAuthController : Controller
    {
        public void Callback(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                this.GetTokens(code);
            }
        }

        public IActionResult GetTokens(string code)
        {
            var tokensFile = "C:\\Users\\skim0\\OneDrive - Manukau Institute of Technology\\Documents\\Level 7(2023)\\AspNetIndustryProject\\AspNetIndustryProject\\Files\\tokens.json";
            var credentialsFile = "C:\\Users\\skim0\\OneDrive - Manukau Institute of Technology\\Documents\\Level 7(2023)\\AspNetIndustryProject\\AspNetIndustryProject\\Files\\credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));


            RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();

            request.AddQueryParameter("code", code);
            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("redirect_uri", "https://localhost:7099/oauth/callback");
            request.AddQueryParameter("grant_type", "authorization_code");

            restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(tokensFile, response.Content);
                return RedirectToAction("Index", "Event");
            }

            return View("Error");
        }

        public IActionResult RefreshToken(string code)
        {
            var tokensFile = "C:\\Users\\skim0\\OneDrive - Manukau Institute of Technology\\Documents\\Level 7(2023)\\AspNetIndustryProject\\AspNetIndustryProject\\Files\\tokens.json";
            var credentialsFile = "C:\\Users\\skim0\\OneDrive - Manukau Institute of Technology\\Documents\\Level 7(2023)\\AspNetIndustryProject\\AspNetIndustryProject\\Files\\credentials.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokensFile));
            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));

            RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();

            //client_id = your_client_id &
            //client_secret = your_client_secret &
            //refresh_token = refresh_token &
            //grant_type = refresh_token

            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());
            request.AddQueryParameter("grant_type", "refresh_token");

            restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(tokensFile, newTokens.ToString());
                return RedirectToAction("Index","Event", new { status = "success"});
            }

            return View("Error");
        }
    }
}

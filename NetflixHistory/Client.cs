using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using RestSharp;

namespace NetflixHistory
{
    public class Client
    {

        protected RestClient _webClient = new RestClient("https://www.netflix.com/");
        protected RestClient _apiClient = new RestClient("https://api-global.netflix.com/");

        public Client(string email, string password, string profile)
        {

            CookieContainer cookieContainer = new CookieContainer();

            _webClient.CookieContainer = cookieContainer;
            _apiClient.CookieContainer = cookieContainer;

            // for the web client, we want to disable all deserialization since RS butchers HTML
            _webClient.ClearHandlers();

            // for some reason our API responses are returned as plain text, so we need to manually treat those as JSON
            _apiClient.AddHandler("text/plain", new RestSharp.Deserializers.JsonDeserializer());

            Login(email, password, profile);

        }

        public ViewingHistory History()
        {

            var request = new RestRequest("/WiViewingActivity");
            IRestResponse response = _webClient.Execute(request);

            var xsrf = ParseXsrfToken(response.Content);

            var historyRequest = new RestRequest("/desktop/account/viewinghistory.1");
            historyRequest.AddParameter("languages", "en-US");
            historyRequest.AddParameter("authURL", xsrf);

            IRestResponse<ViewingHistory> historyResponse = _apiClient.Execute<ViewingHistory>(historyRequest);

            return historyResponse.Data;

        }

        protected void Login(string email, string password, string profile)
        {

            // we could implement our own authenticator, i guess, but screw it...
            var request = new RestRequest("/Login", Method.GET);
            request.AddParameter("locale", "en-US");

            IRestResponse response = _webClient.Execute(request);

            var authUrl = ParseAuthUrl(response.Content);

            // now POST to the same page
            var loginRequest = new RestRequest("/Login", Method.POST);
            loginRequest.AddParameter("locale", "en-US", ParameterType.QueryString);
            loginRequest.AddParameter("authURL", authUrl);
            loginRequest.AddParameter("email", email);
            loginRequest.AddParameter("password", password);
            loginRequest.AddParameter("RememberMe", "on");

            loginRequest.AddHeader("Content-Type", "appilcation/x-www-form-urlencoded");

            IRestResponse loginResponse = _webClient.Execute(loginRequest);

            SwitchProfile(loginResponse.Content, profile);

        }

        protected void SwitchProfile(string response, string profile)
        {

            var xsrf = ParseXsrfToken(response);
            var token = ParseProfileToken(response, profile);

            var switchRequest = new RestRequest("/desktop/account/profiles/switch");
            switchRequest.AddParameter("authURL", xsrf);
            switchRequest.AddParameter("switchProfileGuid", token);

            IRestResponse switchResponse = _apiClient.Execute(switchRequest);

        }

        protected string ParseAuthUrl(string response)
        {

            var regex = new Regex("name=\"authURL\" value=\"([^\"]+)\"");

            Match m = regex.Match(response);

            return m.Groups[1].Captures[0].Value;

        }

        protected string ParseXsrfToken(string response)
        {

            var regex = new Regex("\"xsrf\":\"([^\"]+)\"");

            Match m = regex.Match(response);

            return m.Groups[1].Captures[0].Value;

        }

        protected string ParseProfileToken(string response, string profile)
        {

            var pattern = String.Format("\"profileName\":\"{0}\".*?\"token\":\"(\\w+)\"", profile);
            var regex = new Regex(pattern);

            Match m = regex.Match(response);

            return m.Groups[1].Captures[0].Value;

        }

    }
}

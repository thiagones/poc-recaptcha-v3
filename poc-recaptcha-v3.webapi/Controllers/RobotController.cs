using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace poc_recaptcha_v3.webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RobotController : ControllerBase
    {
        private readonly ILogger<RobotController> _logger;

        public RobotController(ILogger<RobotController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get(string token)
        {
            string secret = "##YOUR_SECRET_KEY##";

            bool validRequest = ValidateChallengeCaptcha(secret, token);

            if (!validRequest)
            {
                return "Eu sou um robô.";
            }

            return "Eu não sou um robô.";
        }

        public bool ValidateChallengeCaptcha(string secret, string token)
        {
            using (var client = new HttpClient())
            {
                string data = $"secret={secret}&response={token}";

                client.BaseAddress = new Uri("https://www.google.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsync("recaptcha/api/siteverify", new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                string responseString = response.Content.ReadAsStringAsync().Result;

                dynamic responseDesirialized = JsonConvert.DeserializeObject<dynamic>(responseString);

                if (responseDesirialized.success != true)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

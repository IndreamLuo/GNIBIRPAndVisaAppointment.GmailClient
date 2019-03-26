using System.Collections.Generic;
using System.Net.Http;

namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public class ConfigurationManager
    {
        readonly string HomeUrl;
        readonly string GetUrl;
        readonly string SetUrl;

        public ConfigurationManager(IConfiguration configuration)
        {
            HomeUrl = configuration["HomeUrl"];
            GetUrl = HomeUrl + "Api/Configuration/Get";
            SetUrl = HomeUrl + "Api/Configuration/Set";
        }

        public string this[string key]
        {
            get
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.PostAsync(GetUrl, new FormUrlEncodedContent(new []
                    {
                        new KeyValuePair<string, string>("area", "Gmail"),
                        new KeyValuePair<string, string>("key", key)
                    }))
                    .Result;

                    var result = response.Content.ReadAsStringAsync().Result;

                    return result;
                }
            }
            set
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.PostAsync(SetUrl, new FormUrlEncodedContent(new []
                    {
                        new KeyValuePair<string, string>("area", "Gmail"),
                        new KeyValuePair<string, string>("key", key),
                        new KeyValuePair<string, string>("value", value)
                    }))
                    .Wait();
                }
            }
        }
    }
}
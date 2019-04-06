using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Gmail.v1.UsersResource.MessagesResource.GetRequest;

namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public class GmailApplication
    {
        readonly OAuthHelper OAuthHelper;
        readonly ConfigurationManager ConfigurationManager;
        readonly string HomeApiToken;
        readonly string SubmitAppointmentLetterUrl;
        readonly string NotUploadedLabelName;
        public GmailApplication(OAuthHelper oAuthHelper, ConfigurationManager configurationManager, IConfiguration configuration)
        {
            OAuthHelper = oAuthHelper;
            ConfigurationManager = configurationManager;
            HomeApiToken = configuration["HomeApiToken"];
            SubmitAppointmentLetterUrl = configuration["HomeUrl"] + "Api/AppointmentLetter/Submit";
            NotUploadedLabelName = configuration["NotUploadedLabelName"];
        }

        public async Task GetNewestAppointmentLetters()
        {
            var credential = await OAuthHelper.GetCredentialAsync();

            using (var gmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GNIBIRPVisa",
            }))
            {
                var getLabelsRequest = gmailService.Users.Labels.List("me");
                var labels = getLabelsRequest.Execute().Labels;
                var label = labels.First(item => item.Name == NotUploadedLabelName);

                var listRequest = gmailService.Users.Messages.List("me");
                listRequest.Q = $"label:{NotUploadedLabelName}";
                
                var listResponse = listRequest.Execute();
                var messages = listResponse.Messages;

                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        var getRequest = gmailService.Users.Messages.Get("me", message.Id);
                        var appointmentEmail = getRequest.Execute();

                        string base64UrlMessage = appointmentEmail.Payload.Body.Data
                                .Replace('_', '/')
                                .Replace('-', '+');

                        switch(appointmentEmail.Payload.Body.Data.Length % 4) {
                            case 2: base64UrlMessage += "=="; break;
                            case 3: base64UrlMessage += "="; break;
                        }

                        var emailMessageBytes = Convert.FromBase64String(base64UrlMessage);
                        var emailMessage = Encoding.ASCII.GetString(emailMessageBytes);

                        if (await SubmitAppointmentLetter(message.Id, emailMessage))
                        {
                            var removeLabelRequest = gmailService.Users.Messages.Modify(new ModifyMessageRequest
                            {
                                RemoveLabelIds = new [] { label.Id }
                            }, "me", message.Id);
                            var removedLabelMessage = removeLabelRequest.Execute();
                        }
                    }
                }
            }
        }

        public async Task<bool> SubmitAppointmentLetter(string id, string message)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(SubmitAppointmentLetterUrl, new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("token", HomeApiToken),
                    new KeyValuePair<string, string>("id", id),
                    new KeyValuePair<string, string>("message", message)
                }));

                return response.StatusCode == HttpStatusCode.Accepted;
            }
        }
    }
}
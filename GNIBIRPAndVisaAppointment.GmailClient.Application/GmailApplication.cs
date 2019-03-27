using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using System;
using System.Threading.Tasks;

namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public class GmailApplication
    {
        readonly OAuthHelper OAuthHelper;
        readonly ConfigurationManager ConfigurationManager;
        public GmailApplication(OAuthHelper oAuthHelper, ConfigurationManager configurationManager)
        {
            OAuthHelper = oAuthHelper;
            ConfigurationManager = configurationManager;
        }

        const string LastGetAppointmentLetters = "LastGetAppointmentLetters";

        public async Task GetNewestAppointmentLetters()
        {
            DateTime lastGetAppointmentLetters;
            if (!DateTime.TryParse(ConfigurationManager[LastGetAppointmentLetters], out lastGetAppointmentLetters))
            {
                lastGetAppointmentLetters = new DateTime(2018, 01, 01);
            }

            var credential = await OAuthHelper.GetCredentialAsync();

            using (var gmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GNIBIRPVisa",
            }))
            {
                var listRequest = gmailService.Users.Messages.List("me");
                var messages = listRequest.Execute().Messages;
            }
        }
    }
}
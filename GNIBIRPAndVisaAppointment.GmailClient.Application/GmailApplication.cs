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
            var lastGetAppointmentLetters = Convert.ToDateTime(ConfigurationManager[LastGetAppointmentLetters]);

            var credential = await OAuthHelper.GetCredentialAsync();
        }
    }
}
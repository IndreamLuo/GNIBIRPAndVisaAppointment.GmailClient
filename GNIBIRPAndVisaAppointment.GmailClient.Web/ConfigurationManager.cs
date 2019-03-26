using Microsoft.Extensions.Configuration;

namespace GNIBIRPAndVisaAppointment.GmailClient.Web
{
    public class ConfigurationManager : GmailClient.Application.IConfiguration
    {
        readonly IConfiguration Configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string this[string key] => Configuration[key];
    }
}
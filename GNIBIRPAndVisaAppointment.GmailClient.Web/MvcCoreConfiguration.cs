using Microsoft.Extensions.Configuration;

namespace GNIBIRPAndVisaAppointment.GmailClient.Web
{
    public class MvcCoreConfiguration : GmailClient.Application.IConfiguration
    {
        readonly IConfiguration Configuration;

        public MvcCoreConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string this[string key] => Configuration[key];
    }
}
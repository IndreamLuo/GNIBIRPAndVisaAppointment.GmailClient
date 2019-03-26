namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public interface IConfiguration
    {
        string this[string key] { get; }
    }
}
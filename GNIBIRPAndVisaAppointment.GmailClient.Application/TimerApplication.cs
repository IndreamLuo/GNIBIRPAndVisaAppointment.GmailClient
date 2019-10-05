using System.Threading.Tasks;
using System.Timers;

namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public class TimerApplication
    {
        readonly GmailApplication GmailApplication;
        readonly Timer Timer;
        public TimerApplication(GmailApplication gmailApplication)
        {
            GmailApplication = gmailApplication;
            Timer = new Timer();
            Timer.Interval = 300000;
            Timer.Elapsed += (sender, e) => Tick();
        }
        
        public bool IsEnabled => Timer.Enabled;

        public async Task Start()
        {
            await Tick();
            Timer.Start();
        }

        public async Task Stop()
        {
            Timer.Stop();
        }

        private async Task Tick()
        {
            await GmailApplication.GetNewestAppointmentLetters();
        }
    }
}
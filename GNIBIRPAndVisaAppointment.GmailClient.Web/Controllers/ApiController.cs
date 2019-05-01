using System;
using System.Threading.Tasks;
using GNIBIRPAndVisaAppointment.GmailClient.Application;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Mvc;

namespace GNIBIRPAndVisaAppointment.GmailClient.Web.Controllers
{
    public class ApiController : Controller
    {
        readonly GmailApplication GmailApplication;

        public ApiController(GmailApplication gmailApplication)
        {
            GmailApplication = gmailApplication;
        }

        [HttpPost]
        public async Task<IActionResult> Send(string to, string subject, string body)
        {
            GmailApplication.SendAsync(to, subject, body);

            return Accepted();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GNIBIRPAndVisaAppointment.GmailClient.Web.Models;
using GNIBIRPAndVisaAppointment.GmailClient.Application;

namespace GNIBIRPAndVisaAppointment.GmailClient.Web.Controllers
{
    public class HomeController : Controller
    {
        GmailApplication GmailApplication;
        public HomeController(GmailApplication gmailApplication)
        {
            GmailApplication = gmailApplication;
        }

        public async Task<IActionResult> Index()
        {
            await GmailApplication.GetNewestAppointmentLetters();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

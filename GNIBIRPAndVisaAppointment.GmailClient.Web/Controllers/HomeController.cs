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
        TimerApplication TimerApplication;
        public HomeController(TimerApplication timerApplication)
        {
            TimerApplication = timerApplication;
        }

        public IActionResult Index()
        {
            ViewBag.IsTimerEnabled = TimerApplication.IsEnabled;

            return View();
        }

        public async Task<IActionResult> StartTimer()
        {
            await TimerApplication.Start();
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

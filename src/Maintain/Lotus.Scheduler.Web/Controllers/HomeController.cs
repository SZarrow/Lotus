using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lotus.Scheduler.Web.Models;
using Lotus.Scheduler.Web.Services;

namespace Lotus.Scheduler.Web.Controllers
{
    public class HomeController : Controller
    {
        private ScheduleService _service = new ScheduleService();

        public IActionResult Index()
        {
            //ViewData["JobList"] = _service.GetJobListItems(1, 10);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

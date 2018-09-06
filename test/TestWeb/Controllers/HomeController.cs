using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lotus.Data.MongoDb;
using Lotus.Logging;
using Microsoft.AspNetCore.Mvc;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        private static ILogger _logger = LogManager.GetLogger();
        private MongoDbContext _context;

        public HomeController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //_logger.Log("Log");
            //_logger.Warn("Warn");

            _logger.Error("Error");
            _logger.Error(new ArgumentNullException("_logger"), "Error");
            return Content("<h1>Index</h1>", "text/html");
        }
    }
}
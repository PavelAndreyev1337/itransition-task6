using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Task6.Models;

namespace itr5.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public GameController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index([FromQuery] string gameId, [FromQuery] string gameName)
        {
            // _logger.LogWarning(gameId);

            // im using player model just to pass gameId to view
            // because for some stupid reason i cant just pass System.String object
            PlayerModel pidor = new PlayerModel(gameId);
            pidor.GameId = gameName;

            return View(pidor);
        }

        public IActionResult Find()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Exchange_Art.Models;
using Exchange_Art.Data;

namespace Exchange_Art.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFlipChain _flipChain;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IFlipChain flipChain)
        {
            _logger = logger;
            _context = context;
            _flipChain = flipChain;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Art.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UserOverview()
        {
            List<ApplicationUser> userList = _context.Users.ToList();

            return View(userList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Exchange_Art.Models;

namespace Exchange_Art.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        // GET:
        // Your Art page
        [Authorize]
        public async Task<IActionResult> YourArt()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);

            ArtOwners artowner = new ArtOwners();

            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);

            artowner.ArtOwner = LoggedInUser;

            // Query for ArtOwners in 'Art' table
            var owners = from o in _context.Art
                         where o.OwnerName.Equals(LoggedInUser.UserName)
                         select o;
            artowner.ArtPieces = owners;

            // Check if ArtOwner object is not NULL
            if (artowner != null)
                return View(artowner);
            else

                return RedirectToAction("Index");
        }
    }
}
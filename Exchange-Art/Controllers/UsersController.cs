using System.Threading.Tasks;
using Exchange_Art.Models;
using Exchange_Art.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Exchange_Art.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private IPasswordHasher<ApplicationUser> _passwordHash;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IPasswordHasher<ApplicationUser> passwordHash)
        {
            _userManager = userManager;
            _context = context;
            _passwordHash = passwordHash;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        // GET:
        // Create a new User
        public ViewResult Create() => View();

        // POST:
        // Create a new User
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Here the data from the User class is used,
                // to create an actual user from the ApplicationUser class,
                // that derives from the IdentityUser class.
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        // GET:
        // Update a user
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            ArtOwners artowner = new ArtOwners();
            
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            artowner.ArtOwner = user;

            // Query for ArtOwners in 'Art' table
            var owners = from o in _context.Art
                         where o.OwnerName.Equals(user.UserName)
                         select o;

            artowner.ArtPieces = owners;

            if (artowner != null)
                return View(artowner);
            else
                return RedirectToAction("Index");
        }

        // POST:
        // Update a user
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = _passwordHash.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        // GET: Users/Delete/23d3d-d3d3f-d33d2-d23f3-35geg
        [Authorize(Roles = Roles.ADMIN_ROLE)]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        // POST:
        // Delete a user
        [HttpPost]
        [Authorize(Roles = Roles.ADMIN_ROLE)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Exchange_Art.Data;
using Exchange_Art.Models;
using System.Globalization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Exchange_Art.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private UserManager<ApplicationUser> _userManager;
        private IFlipChain _flipChain;

        public ArtController(ApplicationDbContext context, INotyfService notyf, UserManager<ApplicationUser> userManager, IFlipChain flipChain)
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;
            _flipChain = flipChain;
        }

        private void CheckExpiredLeases()
        {
            var ArtLeases = (
                from a in _context.ArtLease
                select a).ToList();

            var cultureInfo = new CultureInfo("fr-FR");
            string currentDateString = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime currentDate = DateTime.Parse(currentDateString, cultureInfo, DateTimeStyles.NoCurrentDateDefault);

            foreach (var artlease in ArtLeases)
            {
                var ArtDate = DateTime.Parse(artlease.DateLeaseEnds, cultureInfo, DateTimeStyles.NoCurrentDateDefault);
                int result = DateTime.Compare(ArtDate, currentDate);
                if (result < 0)
                {
                    _context.ArtLease.Remove(artlease); // Removes lease from database
                    Art artPiece = (
                        from ap in _context.Art
                        where ap.Id == artlease.ArtId
                        select ap).FirstOrDefault();
                    artPiece.Leased = false; // Sets Leased boolean to false.
                }
            }
            _context.SaveChanges();
        }

        // GET: Art/Index page
        public async Task<IActionResult> Index()
        {
            return View(await _context.Art.ToListAsync());
        }

        // GET: Art/Leases page
        public async Task<IActionResult> Leases()
        {
            return View(await _context.ArtLease.ToListAsync());
        }

        // GET: Art/ShowBlocks page
        public async Task<IActionResult> ShowBlocks()
        {
            return View(await _context.Blocks.ToListAsync());
        }

        // GET: Art/ShowTransactions page
        public async Task<IActionResult> ShowTransactions()
        {
            return View(await _context.Transactions.ToListAsync());
        }

        // GET: Art/Upload page
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        // GET: Art/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var art = await _context.Art
                .FirstOrDefaultAsync(m => m.Id == id);

            if (art == null)
            {
                return NotFound();
            }

            return View(art);
        }

        // GET: Art/Lease/5
        [Authorize]
        public async Task<IActionResult> Lease(int? id)
        {
            CheckExpiredLeases(); // Deletes a Lease if it is has expired.

            if (id == null)
            {
                return NotFound();
            }

            var art = await _context.Art.FirstOrDefaultAsync(m => m.Id == id);
            var artLease = await _context.ArtLease.FirstOrDefaultAsync(m => m.ArtId == art.Id);
            
            if (art == null)
            {
                return NotFound();
            }

            if(artLease == null)
            {
                ViewBag.UserName = "N.A.";
            }
            else
            {
                ViewBag.UserName = artLease.LeaserName;
            }

            return View(art);
        }

        // GET: Art/DeleteLease/5
        [Authorize(Roles = Roles.ADMIN_ROLE)]
        public async Task<IActionResult> DeleteLease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artLease = await _context.ArtLease.FirstOrDefaultAsync(m => m.Id == id);
            if (artLease == null)
            {
                return NotFound();
            }

            return View(artLease);
        }

        // GET: Art/Delete/5
        [Authorize(Roles = Roles.ADMIN_ROLE)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var art = await _context.Art
                .FirstOrDefaultAsync(m => m.Id == id);
            if (art == null)
            {
                return NotFound();
            }

            return View(art);
        }

        // GET: Art/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var art = await _context.Art.FindAsync(id);
            if (art == null)
            {
                return NotFound();
            }

            var UserId = _userManager.GetUserId(User); // Get logged-in User
            if (art != null && art.UserId == UserId) // Only the Owner may edit his own ArtPiece
            {
                return View(art);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Art/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImageTitle,ImageDescription,ImageData,UserId,OwnerName")] Art art, string LeasePrice)
        {
            if (id != art.Id)
            {
                return NotFound();
            }

            // Convert string LeasePrice back to decimal
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            decimal dLeasePrice = decimal.Parse(LeasePrice, style); // Parse string to decimal
            art.LeasePrice = dLeasePrice; // Store decimal value in Art Model (table)
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(art);
                    await _context.SaveChangesAsync(); // Save changes in Database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtExists(art.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(art);
        }

        // GET: RequestLease
        // Art/Lease/5
        [Authorize]
        public async Task<IActionResult> RequestLease(int? id)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            var UserId = _userManager.GetUserId(currentUser);
            ApplicationUser LoggedInUser = await _userManager.FindByIdAsync(UserId);
            
            if (id == null)
            {
                return NotFound();
            }

            var art = await _context.Art
                .FirstOrDefaultAsync(m => m.Id == id);
            if (art == null)
            {
                return NotFound();
            }

            if (art.OwnerName == LoggedInUser.UserName)
            {
                _notyf.Error("You cannot Lease your own Artpiece!");
                return View("Lease", art);
            }
            else
            {
                return View(art);
            }
        }

        // POST: RequestLease
        [HttpPost]
        public async Task<IActionResult> RequestLease(  int ArtPieceId, 
                                                        int ArtPeriod, 
                                                        string LeaserId, 
                                                        string LeaserName, 
                                                        string LeaseAmount)
        {
            // Reference ArtPiece that is to be leased
            var ArtPiece = await _context.Art.FirstOrDefaultAsync(m => m.Id == ArtPieceId);

            if (ArtPiece == null)
            {
                return NotFound();
            }

            if (ArtPiece.Leased == false) // If ArtPiece is not leased yet
            {
                // Convert string LeaseAmount back to decimal
                NumberStyles style = NumberStyles.AllowDecimalPoint;
                decimal dLeaseAmount = decimal.Parse(LeaseAmount, style);
                dLeaseAmount *= ArtPeriod;

                // Create ArtLease object
                ArtLease LeasedArt = new ArtLease
                {
                    // Fill in all required columns of ArtLease object (Transaction field will be done later)
                    ArtId = ArtPieceId,
                    ArtPieceImageData = ArtPiece.ImageData,
                    ArtDescription = ArtPiece.ImageDescription,
                    LeaserId = LeaserId,
                    LeaserName = LeaserName,
                    LeasePeriodInMonths = ArtPeriod,
                    DateLeaseStarted = DateTime.Today.ToString("d"),
                    DateLeaseEnds = DateTime.Today.AddMonths(ArtPeriod).ToString("d"),
                    OwnerId = ArtPiece.UserId,
                    OwnerName = ArtPiece.OwnerName
                };

                // Set boolean 'Leased' to TRUE in ArtPiece requested for Lease (Art model)
                ArtPiece.Leased = true;

                // Reference LeaseRequester and Owner of the ArtPiece
                ApplicationUser LeaseRequester = await _userManager.FindByIdAsync(LeaserId);
                ApplicationUser ArtOwner = await _userManager.FindByIdAsync(ArtPiece.UserId);

                // Reference wallet addresses
                string fromAddress = (
                    from a in _context.Wallets
                    where a.Username == LeaseRequester.UserName
                    select a.publicAddress).FirstOrDefault();

                string toAddress = (
                    from a in _context.Wallets
                    where a.Username == ArtOwner.UserName
                    select a.publicAddress).FirstOrDefault();

                // Create transaction and store it in the ArtLease object
                LeasedArt.Transaction = _flipChain.CreateTransaction(fromAddress, toAddress, dLeaseAmount);

                // Store ArtLease record in database
                _context.ArtLease.Add(LeasedArt);

                // Save changes to database
                await _context.SaveChangesAsync();

                // Send ViewBag message with LeaseRequest data back to View
                _notyf.Success("Lease request submitted!", 10);
            }
            else
            {
                _notyf.Error("Lease request failed, ArtPiece already taken!", 10);
            }

            return View(ArtPiece);
        }

        public async Task<IActionResult> CreateGenesisBlock()
        {
            _flipChain.AddGenesisBlock();

            return View("ShowBlocks", await _context.Blocks.ToListAsync());
        }

        public async Task<IActionResult> ProcessPendingTransactions()
        {
            _flipChain.ProcessPendingTransactions();

            return View("ShowTransactions", await _context.Transactions.ToListAsync());
        }

        // POST: Art/Upload
        [HttpPost]
        public async Task<IActionResult> UploadArt(string title1, string description1, string owner1, string username1)
        {
            foreach (var file in Request.Form.Files) // In case of multiple files
            {
                Art ArtImg = new Art();
                ArtImg.ImageTitle = title1;
                ArtImg.ImageDescription = description1;
                ArtImg.UserId = owner1;
                ArtImg.OwnerName = username1;

                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                ArtImg.ImageData = ms.ToArray();

                ms.Close();
                ms.Dispose();

                // Store ArtImg in Art table
                _context.Art.Add(ArtImg);
                
                // Save changes to database
                await _context.SaveChangesAsync();
            }

            Art ArtImage = _context.Art.OrderByDescending(i => i.Id).FirstOrDefault();

            string imageBase64Data = Convert.ToBase64String(ArtImage.ImageData);
            string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);

            ViewBag.ImageTitle = ArtImage.ImageTitle;
            ViewBag.ImageDataUrl = imageDataURL;
            ViewBag.Message = $"Artpiece {ArtImage.ImageTitle} stored in database!";
            
            return View("Upload");
        }

        // POST: RetrieveArt
        [HttpPost]
        public ActionResult RetrieveArt()
        {
            Art ArtImg = _context.Art.OrderByDescending(i => i.Id).FirstOrDefault();
            
            string imageBase64Data = Convert.ToBase64String(ArtImg.ImageData);
            string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
            
            ViewBag.ImageTitle = ArtImg.ImageTitle;
            ViewBag.ImageDataUrl = imageDataURL;
            
            return View("Upload");
        }

        // POST: Art/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var art = await _context.Art.FindAsync(id);
            _context.Art.Remove(art);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Art/DeleteLease/5
        [HttpPost, ActionName("DeleteLease")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLeaseConfirmed(int id)
        {
            // Remove ArtLease record
            var artLease = await _context.ArtLease.FindAsync(id);
            _context.ArtLease.Remove(artLease);

            // Set boolean Leased to FALSE in ArtPiece record
            var art = await _context.Art.FindAsync(artLease.ArtId);
            art.Leased = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Leases));

        }

        private bool ArtExists(int id)
        {
            return _context.Art.Any(e => e.Id == id);
        }
    }
}

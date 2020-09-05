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
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Globalization;

namespace Exchange_Art.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private WalletController _walletController;

        public ArtController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _walletController = new WalletController();
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

            // Convert string LeaseAmount back to decimal
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            decimal dLeaseAmount = decimal.Parse(LeaseAmount, style); // Parse string to decimal

            // Create ArtLease instance
            ArtLease LeasedArt = new ArtLease();

            // Fill in all required columns of ArtLease model (table)
            LeasedArt.ArtId                 = ArtPieceId;
            LeasedArt.ArtPieceImageData     = ArtPiece.ImageData;
            LeasedArt.ArtDescription        = ArtPiece.ImageDescription;
            LeasedArt.LeaserId              = LeaserId;
            LeasedArt.LeaserName            = LeaserName;
            LeasedArt.LeasePeriodInMonths   = ArtPeriod;
            LeasedArt.DateLeaseStarted      = DateTime.Today.ToString("d");
            LeasedArt.DateLeaseEnds         = DateTime.Today.AddMonths(ArtPeriod).ToString("d");
            LeasedArt.CryptoAmount          = dLeaseAmount * ArtPeriod;
            LeasedArt.OwnerId               = ArtPiece.UserId;
            LeasedArt.OwnerName             = ArtPiece.OwnerName;

            // Store ArtLease record in database
            _context.ArtLease.Add(LeasedArt);

            // Set boolean Leased to TRUE in ArtPiece requested for Lease (Art model (table))
            ArtPiece.Leased = true;
            
            // Reference LeaseRequester and Owner of the ArtPiece
            ApplicationUser LeaseRequester = await _userManager.FindByIdAsync(LeaserId);
            ApplicationUser ArtOwner = await _userManager.FindByIdAsync(ArtPiece.UserId);

            // Reference Ethereum account for balance checking
            var LeaseRequesterAccount = new Account(LeaseRequester.WalletPrivateKey);
            var web3 = new Web3(LeaseRequesterAccount, "https://ropsten.infura.io/v3/87f3fd59965241539d6721a231635ea0");
            var etherBalance = await _walletController.GetAccountBalance(LeaseRequesterAccount); // Check Ethererum balance

            if (etherBalance < LeasedArt.CryptoAmount)
            {
                ViewBag.Message = $"Lease Request of Artpiece {ArtPiece.ImageTitle} denied, the Ethereum balance is to low: {etherBalance}";
            }
            else
            {
                // Exchange Ether LeaseAmount between the two Ethereum addresses
                Nethereum.RPC.Eth.DTOs.TransactionReceipt transaction = await _walletController.TransferEther(
                                        LeaseRequester.WalletPrivateKey,
                                        ArtOwner.EtheruemAddress,
                                        LeasedArt.CryptoAmount);

                // Save changes to database
                await _context.SaveChangesAsync();

                // Send ViewBag message with LeaseRequest data back to View
                ViewBag.Message = $"Lease Request of Artpiece {ArtPiece.ImageTitle} submitted for {LeasedArt.CryptoAmount} Ethereum.";
            }

            return View(ArtPiece);
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

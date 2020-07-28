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

namespace Exchange_Art.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ArtController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // GET: Art/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Art/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImageTitle,ImageData")] Art art)
        {
            if (ModelState.IsValid)
            {
                _context.Add(art);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImageTitle,ImageDescription,ImageData,UserId,OwnerName,LeasePrice")] Art art)
        {
            if (id != art.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(art);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> RequestLease(int ArtPieceId,int ArtPeriod, string LeaserId, string LeaserName, double LeaseAmount)
        {
            var ArtPiece = await _context.Art.FirstOrDefaultAsync(m => m.Id == ArtPieceId);
            if (ArtPiece == null)
            {
                return NotFound();
            }

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
            LeasedArt.CryptoAmount          = LeaseAmount * ArtPeriod;
            LeasedArt.OwnerId               = ArtPiece.UserId;
            LeasedArt.OwnerName             = ArtPiece.OwnerName;

            // Store ArtLease record in database
            _context.ArtLease.Add(LeasedArt);

            // Set boolean Leased to TRUE in ArtPiece requested for Lease (Art model (table))
            ArtPiece.Leased = true;

            // Save changes to database
            await _context.SaveChangesAsync();

            // Send ViewBag with ArtImage data back to View
            string imageBase64Data = Convert.ToBase64String(ArtPiece.ImageData);
            string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
            ViewBag.ImageTitle = ArtPiece.ImageTitle;
            ViewBag.ImageDataUrl = imageDataURL;
            ViewBag.Message = "Lease Request Submitted!";

            return View(ArtPiece);
        }

        // POST: UploadArt
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
            ViewBag.Message = "Art piece stored in database!";
            
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
            return RedirectToAction(nameof(Index));

        }

        private bool ArtExists(int id)
        {
            return _context.Art.Any(e => e.Id == id);
        }
    }
}

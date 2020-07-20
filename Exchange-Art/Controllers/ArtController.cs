using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exchange_Art.Models;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization;
using Exchange_Art.Data;

namespace Exchange_Art.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Art
        public async Task<IActionResult> Index()
        {
            return View(await _context.Art.ToListAsync());
        }

        // GET: Art
        public IActionResult Upload()
        {
            return View();
        }

        // GET: Art/Details/5
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
            return View(art);
        }

        // POST: Art/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImageTitle,ImageData")] Art art)
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

                _context.Art.Add(ArtImg);
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

        private bool ArtExists(int id)
        {
            return _context.Art.Any(e => e.Id == id);
        }
    }
}

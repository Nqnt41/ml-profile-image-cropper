using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileSearch.Server.Data;
using ProfileSearch.Server.Models;

namespace ProfileSearch.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ProfileSearchContext _context;

        private readonly CloudinaryContext _cloudinary;

        public ImagesController(ProfileSearchContext context, CloudinaryContext cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Images
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetImages()
        {
            return Ok(await _context.Images.Include(i => i.User).ToListAsync());
        }

        // GET: Images/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        // GET: api/images/getPairs
        [HttpGet("getPairs")]
        public async Task<ActionResult<IEnumerable<object>>> GetPairs()
        {   
            return Ok(await _context.Images.Select(i => new { i.Id, i.Name }).ToListAsync());
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult<Image>> AddImage(Image image)
        {
            if (ModelState.IsValid)
            {
                _context.Add(image);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetImage), new { id = image.Id }, image);
            }
            return BadRequest(ModelState);
        }

        // PUT: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Image image)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(image).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return NoContent();
        }

        // DELETE: Images/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: Get cloudinary details
        [HttpGet("getCloudinaryContext")]
        public ActionResult GetCloudinaryDetails()
        {
            var cloudinaryContext = new
            {
                url = _cloudinary.Url,
                uploadPreset = _cloudinary.UploadPreset
            };
            return Ok(cloudinaryContext);
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }
    }
}

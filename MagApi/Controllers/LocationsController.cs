using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagApi.Models;
using Microsoft.Extensions.Logging;
using MagApi.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace MagApi.Controllers
{
    [Route("api/public/v1/locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<AreasController> _logger;
        private readonly MagDbContext _context;

        public LocationsController(ILogger<AreasController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Locations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            return await _context.Locations
                                .Select(l => new Location() { 
                                    Id = l.Id,
                                    Name = l.Name,
                                    Description = l.Description,
                                    Notes = l.Notes
                                })
                                .OrderBy(l => l.Name)
                                .ToListAsync();
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<ActionResult<Location>> GetLocation(long id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            return new Location() { 
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Notes = location.Notes
            };
        }

        // PUT: api/Locations/5
        [HttpPut("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> PutLocation(long id, Location dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Notes = dto.Notes;
            location.ModifiedBy = HttpContext.User.Identity.Name;
            location.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> DeleteLocation(long id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(long id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}

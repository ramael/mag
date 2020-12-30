using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagApi.Controllers;
using MagApi.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MagApi.Models
{
    [Route("api/public/v1/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ILogger<AreasController> _logger;
        private readonly MagDbContext _context;

        public AreasController(ILogger<AreasController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Areas
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            return await _context.Areas
                                .Select(a => new Area() { 
                                    Id = a.Id,
                                    Name = a.Name,
                                    Description = a.Description,
                                    Notes = a.Notes
                                })
                                .ToListAsync();
        }

        // GET: api/Areas/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Area>> GetArea(long id, [FromQuery(Name = "includelocations")] bool includeLocations)
        {
            var partialQuery = _context.Areas;
            if (includeLocations)
            {
                partialQuery.Include(a => a.Locations);
            }

            var area = await partialQuery.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (area == null)
            {
                return NotFound();
            }

            var dto = new Area()
            {
                Id = area.Id,
                Name = area.Name,
                Description = area.Description,
                Notes = area.Notes
            };

            if (includeLocations) {
                dto.Locations = area.Locations.Select(l => new Location()
                                                {
                                                    Id = l.Id,
                                                    Name = l.Name,
                                                    Description = l.Description,
                                                    Notes = l.Notes
                                                })
                                            .ToList();
            }
            return dto;
        }

        // PUT: api/Areas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> PutArea(long id, Area dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            area.Name = dto.Name;
            area.Description = dto.Description;
            area.Notes = dto.Notes;
            area.ModifiedBy = HttpContext.User.Identity.Name;
            area.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
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

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> DeleteArea(long id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Areas/{id}/Locations
        [HttpPost("{id}/locations")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<ActionResult<Location>> PostAreaLocation(long id, Location dto)
        {
            var exists = await _context.Areas.Where(a => a.Id == id).AnyAsync();
            if (!exists)
            {
                return NotFound("Area not found");
            }

            var location = new LocationModel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Notes = dto.Notes,
                AreaId = id
            };
            location.CreatedBy = HttpContext.User.Identity.Name;
            location.ModifiedBy = HttpContext.User.Identity.Name;

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            return CreatedAtAction(nameof(LocationsController.GetLocation), new { id = location.Id }, location);
        }

        private bool AreaExists(long id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
    }
}

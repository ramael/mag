using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagApi.Models;
using MagApi.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using MagApi.Exceptions;

namespace MagApi.Controllers
{
    [Route("api/public/v1/components")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly ILogger<ComponentsController> _logger;
        private readonly MagDbContext _context;

        public ComponentsController(ILogger<ComponentsController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Components
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Component>>> GetComponents()
        {
            return await _context.Components
                                .Select(c => new Component() { 
                                    Id = c.Id,
                                    Code = c.Code,
                                    Description = c.Description,
                                    Notes = c.Notes
                                })
                                .OrderBy(c => c.Code)
                                .ToListAsync();
        }

        // GET: api/Components/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Component>> GetComponent(long id)
        {
            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }

            return new Component() { 
                Id = component.Id,
                Code = component.Code,
                Description = component.Description,
                Notes = component.Notes
            };
        }

        // PUT: api/Components/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComponent(long id, Component dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }

            component.Code = dto.Code;
            component.Description = dto.Description;
            component.Notes = dto.Notes;
            component.ModifiedBy = HttpContext.User.Identity.Name;
            component.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentExists(id))
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

        // POST: api/Components
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Component>> PostComponent(Component dto)
        {
            var component = new ComponentModel()
            {
                Code = dto.Code,
                Description = dto.Description,
                Notes = dto.Notes
            };
            component.CreatedBy = HttpContext.User.Identity.Name;
            component.ModifiedBy = HttpContext.User.Identity.Name;

            _context.Components.Add(component);

            try 
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = (SqlException)ex.InnerException;
                if (inner.IsUniqueKeyViolation()) {
                    return Conflict("Duplicate id or code");
                }
                throw;
            }

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetComponent", new { id = component.Id }, component);
            return CreatedAtAction(nameof(GetComponent), new { id = component.Id }, dto);
        }

        // DELETE: api/Components/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComponent(long id)
        {
            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }

            _context.Components.Remove(component);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComponentExists(long id)
        {
            return _context.Components.Any(e => e.Id == id);
        }
    }
}

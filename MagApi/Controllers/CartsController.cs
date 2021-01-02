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
    [Route("api/public/v1/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ILogger<CartsController> _logger;
        private readonly MagDbContext _context;

        public CartsController(ILogger<CartsController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts
                                .Select(c => new Cart() { 
                                    Id = c.Id,
                                    SerialNumber = c.SerialNumber,
                                    Status = (Cart.StatusEnum) (int) c.Status
                                })
                                .OrderBy(c => c.SerialNumber)
                                .ToListAsync();
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Cart>> GetCart(long id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            return new Cart() { 
                Id = cart.Id,
                SerialNumber = cart.SerialNumber,
                Status = (Cart.StatusEnum) (int) cart.Status
            };
        }

        // PUT: api/Carts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> PutCart(long id, Cart dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            cart.SerialNumber = dto.SerialNumber;
            cart.Status = (CartModel.StatusEnum)(int)dto.Status;
            cart.ModifiedBy = HttpContext.User.Identity.Name;
            cart.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // POST: api/Carts
        [HttpPost]
        [Authorize(Roles = "CartManager")]
        public async Task<ActionResult<Cart>> PostCart(Cart dto)
        {
            var cart = new CartModel()
            {
                SerialNumber = dto.SerialNumber,
                Status = (CartModel.StatusEnum)(int)dto.Status
            };
            cart.CreatedBy = HttpContext.User.Identity.Name;
            cart.ModifiedBy = HttpContext.User.Identity.Name;

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> DeleteCart(long id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(long id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}

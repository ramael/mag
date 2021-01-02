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
    [Route("api/public/v1/loadedcarts")]
    [ApiController]
    public class LoadedCartsController : ControllerBase
    {
        private readonly ILogger<LoadedCartsController> _logger;
        private readonly MagDbContext _context;

        public LoadedCartsController(ILogger<LoadedCartsController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/LoadedCarts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LoadedCart>>> GetLoadedCarts()
        {
            return await _context.LoadedCarts
                                .Include(lc => lc.Cart)
                                .Include(lc => lc.Location)
                                .Select(lc => new LoadedCart()
                                    {
                                        Id = lc.Id,
                                        Year = lc.Year,
                                        Progressive = lc.Progressive,
                                        Description = lc.Description,
                                        DateIn = lc.DateIn,
                                        DateOut = lc.DateOut,
                                        CartId = lc.CartId,
                                        Cart = new Cart() { 
                                            Id = lc.Cart.Id,
                                            SerialNumber = lc.Cart.SerialNumber,
                                            Status = (Cart.StatusEnum)(int)lc.Cart.Status
                                        },
                                        LocationId = lc.LocationId,
                                        Location = new Location() { 
                                            Id = lc.Location.Id,
                                            Name = lc.Location.Name,
                                            Description = lc.Location.Description,
                                            Notes = lc.Location.Notes
                                        }                             
                                    }
                                )
                                .OrderBy(lc => lc.Year).ThenBy(lc => lc.Progressive)
                                .ToListAsync();
        }

        // GET: api/LoadedCarts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LoadedCart>> GetLoadedCart(long id, [FromQuery(Name = "includedetails")] bool includeDetails)
        {
            var partialQuery = _context.LoadedCarts.Include(lc => lc.Cart)
                                                    .Include(lc => lc.Location);
            if (includeDetails)
            {
                partialQuery.Include(lc => lc.LoadedCartDetails)
                            .ThenInclude(lcd => lcd.Component);
            }

            var loadedCart = await partialQuery.Where(lc => lc.Id == id)
                                                .FirstOrDefaultAsync();
            if (loadedCart == null)
            {
                return NotFound();
            }

            var dto = new LoadedCart() {
                Id = loadedCart.Id,
                Year = loadedCart.Year,
                Progressive = loadedCart.Progressive,
                Description = loadedCart.Description,
                DateIn = loadedCart.DateIn,
                DateOut = loadedCart.DateOut,
                CartId = loadedCart.CartId,
                Cart = new Cart() {
                    Id = loadedCart.Cart.Id,
                    SerialNumber = loadedCart.Cart.SerialNumber,
                    Status = (Cart.StatusEnum)(int)loadedCart.Cart.Status
                },
                LocationId = loadedCart.LocationId,
                Location = new Location() {
                    Id = loadedCart.Location.Id,
                    Name = loadedCart.Location.Name,
                    Description = loadedCart.Location.Description,
                    Notes = loadedCart.Location.Notes
                }
                                    
            };

            if (includeDetails)
            {
                dto.LoadedCartDetails = loadedCart.LoadedCartDetails.Select(lcd => new LoadedCartDetail()
                                                                    {
                                                                        Id = lcd.Id,
                                                                        ComponentId = lcd.ComponentId,
                                                                        Component = new Component() { 
                                                                            Id = lcd.Component.Id,
                                                                            Code = lcd.Component.Code,
                                                                            Description = lcd.Component.Description,
                                                                            Notes = lcd.Component.Notes
                                                                        },
                                                                        LoadedCartId = lcd.LoadedCartId,
                                                                        Notes = lcd.Notes
                                                                    })
                                                                    .ToList();
            }
            return dto;
        }

        // PUT: api/LoadedCarts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> PutLoadedCart(long id, LoadedCart dto)
        {
            // We will modify only Description and Location
            var loadedCart = await _context.LoadedCarts.FindAsync(id);
            if (loadedCart == null)
            {
                return NotFound();
            }

            if (dto.LocationId != 0 && dto.LocationId != loadedCart.LocationId)
                loadedCart.LocationId = dto.LocationId;
            if (dto.Description != loadedCart.Description)
                loadedCart.Description = dto.Description;
            loadedCart.ModifiedBy = HttpContext.User.Identity.Name;
            loadedCart.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoadedCartExists(id))
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

        // PUT: api/LoadedCarts/5
        [HttpPut("{id}/checkout")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> CheckoutLoadedCart(long id)
        {
            // We will modify DateOut only and make the bound Cart available again
            var loadedCart = await _context.LoadedCarts.Include(lc => lc.Cart)
                                                        .Where(lc => lc.Id == id)
                                                        .FirstOrDefaultAsync();
            if (loadedCart == null)
            {
                return NotFound();
            }

            loadedCart.DateOut = DateTime.Now;
            loadedCart.Cart.Status = CartModel.StatusEnum.Available;
            loadedCart.ModifiedBy = HttpContext.User.Identity.Name;
            loadedCart.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoadedCartExists(id))
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

        // POST: api/LoadedCarts
        [HttpPost]
        [Authorize(Roles = "CartManager")]
        public async Task<ActionResult<LoadedCartModel>> PostLoadedCart(LoadedCart dto)
        {
            var cart = await _context.Carts.FindAsync(dto.CartId);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }
            if (cart.Status != CartModel.StatusEnum.Available)
            {
                return BadRequest("Cart not available");
            }

            cart.Status = CartModel.StatusEnum.NotAvailable;
            
            var location = await _context.Locations.FindAsync(dto.LocationId);
            if (location == null)
            {
                return NotFound("Location not found");
            }

            var user = HttpContext.User.Identity.Name;
            var loadedCart = new LoadedCartModel()
            {
                DateIn = dto.DateIn,
                Description = dto.Description,
                CartId = cart.Id,
                LocationId = location.Id
            };
            loadedCart.CreatedBy = user;
            loadedCart.ModifiedBy = user;

            if (dto.LoadedCartDetails != null && dto.LoadedCartDetails.Count > 0)
            {
                loadedCart.LoadedCartDetails= dto.LoadedCartDetails.Select(lcdDto => new LoadedCartDetailModel()
                                                                        {
                                                                            ComponentId = lcdDto.ComponentId,
                                                                            LoadedCartId = lcdDto.LoadedCartId,
                                                                            Notes = lcdDto.Notes,
                                                                            CreatedBy = user,
                                                                            ModifiedBy = user
                                                                        })
                                                                    .ToList();
            }

            _context.LoadedCarts.Add(loadedCart);
            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetLoadedCart", new { id = loadedCart.Id }, loadedCart);
            return CreatedAtAction(nameof(GetLoadedCart), new { id = loadedCart.Id, includeDetails = false }, loadedCart);
            
        }

        // GET: api/LoadedCarts/5/Details
        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LoadedCartDetail>>> GetLoadedCartsDetails(long id)
        {
            return await _context.LoadedCartDetails
                                .Include(lcd => lcd.Component)
                                .Where(lcd => lcd.LoadedCartId == id)
                                .Select(lcd => new LoadedCartDetail()
                                {
                                    Id = lcd.Id,
                                    ComponentId = lcd.ComponentId,
                                    Component = new Component() { 
                                        Id = lcd.Component.Id,
                                        Code = lcd.Component.Code,
                                        Description = lcd.Component.Description,
                                        Notes = lcd.Component.Notes
                                    },
                                    LoadedCartId = lcd.LoadedCartId,
                                    Notes = lcd.Notes
                                }
                                )
                                .ToListAsync();
        }

        // GET: api/LoadedCarts/5/Details/7
        [HttpGet("{id}/details/{detailid}")]
        [Authorize]
        public async Task<ActionResult<LoadedCartDetail>> GetLoadedCartDetail(long id, long detailid)
        {
            var loadedCartDetail = await _context.LoadedCartDetails
                                                .Include(lcd => lcd.Component)
                                                .Where(lcd => lcd.Id == detailid)
                                                .FirstOrDefaultAsync();
            if (loadedCartDetail == null)
            {
                return NotFound();
            }

            var dto = new LoadedCartDetail() { 
                Id = loadedCartDetail.Id,
                ComponentId = loadedCartDetail.ComponentId,
                Component = new Component() { 
                    Id = loadedCartDetail.Component.Id,
                    Code = loadedCartDetail.Component.Code,
                    Description = loadedCartDetail.Component.Description,
                    Notes = loadedCartDetail.Component.Notes
                },
                LoadedCartId = loadedCartDetail.LoadedCartId,
                Notes = loadedCartDetail.Notes
            };
            return dto;
        }

        // PUT: api/LoadedCarts/5/Details/7
        [HttpPut("{id}/details/{detailid}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> PutLoadedCartDetail(long id, long detailid, LoadedCartDetail dto)
        {
            // We will modify only Description and Location
            var loadedCartDetail = await _context.LoadedCartDetails.FindAsync(id);
            if (loadedCartDetail == null)
            {
                return NotFound();
            }

            if (dto.ComponentId != 0 && dto.ComponentId != loadedCartDetail.ComponentId)
                loadedCartDetail.ComponentId = dto.ComponentId;
            if (dto.Notes != loadedCartDetail.Notes)
                loadedCartDetail.Notes = dto.Notes;
            loadedCartDetail.ModifiedBy = HttpContext.User.Identity.Name;
            loadedCartDetail.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoadedCartExists(id))
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

        // POST: api/LoadedCarts/5/Details
        [HttpPost("{id}/details")]
        [Authorize(Roles = "CartManager")]
        public async Task<ActionResult<LoadedCartDetailModel>> PostLoadedCartDetail(long id, LoadedCartDetail dto)
        {
            var exists = await _context.LoadedCarts.Where(lc => lc.Id == id).AnyAsync();
            if (!exists)
            {
                return NotFound();
            }

            var lcd = new LoadedCartDetailModel()
            { 
                LoadedCartId = id,
                ComponentId = dto.ComponentId,
                Notes = dto.Notes
            };
            lcd.CreatedBy = HttpContext.User.Identity.Name;
            lcd.ModifiedBy = HttpContext.User.Identity.Name;
            
            _context.LoadedCartDetails.Add(lcd);

            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetLoadedCart", new { id = loadedCart.Id }, loadedCart);
            return CreatedAtAction(nameof(GetLoadedCartDetail), new { id = id, detailid = lcd.Id }, lcd);

        }

        // DELETE: api/LoadedCarts/5/Details/7
        [HttpDelete("{id}/details/{detailid}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> DeleteLoadedCartDetail(long id, long detailid)
        {
            var lcd = await _context.LoadedCartDetails.FindAsync(detailid);
            if (lcd == null)
            {
                return NotFound();
            }

            _context.LoadedCartDetails.Remove(lcd);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoadedCartExists(long id)
        {
            return _context.LoadedCarts.Any(e => e.Id == id);
        }
    }
}

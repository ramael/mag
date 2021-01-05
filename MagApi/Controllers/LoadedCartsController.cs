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
using Microsoft.Data.SqlClient;
using MagApi.Exceptions;

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
                                .ThenInclude(l => l.Area)
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
                                        AreaId = lc.Location.AreaId,
                                        Area = new Area() {
                                            Id = lc.Location.Area.Id,
                                            Name = lc.Location.Area.Name,
                                            Description = lc.Location.Area.Description,
                                            Notes = lc.Location.Area.Notes
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
            LoadedCartModel loadedCart = null;
            if (includeDetails)
            {
                loadedCart = await _context.LoadedCarts.Include(lc => lc.Cart)
                                                    .Include(lc => lc.Location)
                                                    .ThenInclude(l => l.Area)
                                                    .Include(lc => lc.LoadedCartDetails)
                                                    .ThenInclude(lcd => lcd.Component)
                                                    .Where(lc => lc.Id == id)
                                                    .FirstOrDefaultAsync();
            }
            else 
            {
                loadedCart = await _context.LoadedCarts.Include(lc => lc.Cart)
                                                        .Include(lc => lc.Location)
                                                        .ThenInclude(l => l.Area)
                                                        .Where(lc => lc.Id == id)
                                                        .FirstOrDefaultAsync();
            }
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
                AreaId = loadedCart.Location.AreaId,
                Area = new Area() {
                    Id = loadedCart.Location.Area.Id,
                    Name = loadedCart.Location.Area.Name,
                    Description = loadedCart.Location.Area.Description,
                    Notes = loadedCart.Location.Area.Notes
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
            return Ok(dto);
        }

        // PUT: api/LoadedCarts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "CartManager")]
        public async Task<IActionResult> PutLoadedCart(long id, LoadedCart dto)
        {
            // We will modify only Description and Location
            var loadedCart = await _context.LoadedCarts.FindAsync(id);;
            if (loadedCart == null)
            {
                return NotFound();
            }

            var user = HttpContext.User.Identity.Name;
            var now = DateTime.Now;
            if (dto.LocationId != 0 && dto.LocationId != loadedCart.LocationId)
                loadedCart.LocationId = dto.LocationId;
            if (dto.Description != loadedCart.Description)
                loadedCart.Description = dto.Description;
            loadedCart.ModifiedBy = user;
            loadedCart.ModifiedOn = now;

            if (dto.LoadedCartDetails != null && dto.LoadedCartDetails.Count > 0)
            {
                var newItems = dto.LoadedCartDetails.Where(lcd => lcd.Status == LoadedCartDetail.StatusEnum.New)
                                                    .Select(lcd => new LoadedCartDetailModel() 
                                                    { 
                                                        Id = 0,
                                                        ComponentId = lcd.ComponentId,
                                                        LoadedCartId = lcd.LoadedCartId,
                                                        Notes = lcd.Notes,
                                                        CreatedBy = user,
                                                        ModifiedBy = user
                                                    })
                                                    .ToList();
                var modItems = dto.LoadedCartDetails.Where(lcd => lcd.Status == LoadedCartDetail.StatusEnum.Modified)
                                                    .ToList();
                var delItems = dto.LoadedCartDetails.Where(lcd => lcd.Status == LoadedCartDetail.StatusEnum.Deleted)
                                                    .Select(lcd => new LoadedCartDetailModel()
                                                    {
                                                        Id = lcd.Id,
                                                        ComponentId = lcd.ComponentId,
                                                        LoadedCartId = lcd.LoadedCartId
                                                    })
                                                    .ToList();
                if (newItems.Count > 0)
                    _context.LoadedCartDetails.AddRange(newItems);

                if (modItems.Count > 0) {
                    foreach (LoadedCartDetail lcd in modItems) {
                        var lcdModel = await _context.LoadedCartDetails.FindAsync(lcd.Id);
                        if (lcdModel != null) {
                            lcdModel.Notes = lcd.Notes;
                            lcdModel.ModifiedBy = user;
                            lcdModel.ModifiedOn = now;
                        }
                    }
                }
                
                if (delItems.Count > 0)
                    _context.LoadedCartDetails.RemoveRange(delItems);

            }

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

            var now = DateTime.Now;
            loadedCart.DateOut = now;
            loadedCart.Cart.Status = CartModel.StatusEnum.Available;
            loadedCart.Cart.ModifiedBy = HttpContext.User.Identity.Name;
            loadedCart.Cart.ModifiedOn = now;
            loadedCart.ModifiedBy = HttpContext.User.Identity.Name;
            loadedCart.ModifiedOn = now;

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
                Year = dto.Year,
                Progressive = dto.Progressive,
                Description = dto.Description,
                DateIn = dto.DateIn,
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = (SqlException)ex.InnerException;
                if (inner.IsUniqueKeyViolation())
                {
                    return Conflict("Duplicate id or year/progressive");
                }
                throw;
            }

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetLoadedCart", new { id = loadedCart.Id }, loadedCart);
            return CreatedAtAction(nameof(GetLoadedCart), new { id = loadedCart.Id, includeDetails = false }, dto);
            
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = (SqlException)ex.InnerException;
                if (inner.IsUniqueKeyViolation())
                {
                    return Conflict("Duplicate id");
                }
                throw;
            }

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetLoadedCart", new { id = loadedCart.Id }, loadedCart);
            return CreatedAtAction(nameof(GetLoadedCartDetail), new { id = id, detailid = lcd.Id }, dto);

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

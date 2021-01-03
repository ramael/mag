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
    [Route("api/public/v1/warehouses")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly ILogger<WarehousesController> _logger;
        private readonly MagDbContext _context;

        public WarehousesController(ILogger<WarehousesController> logger, MagDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Warehouses
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            return await _context.Warehouses
                                .Select(w => new Warehouse() { 
                                    Id = w.Id,
                                    Name = w.Name,
                                    Description = w.Description,
                                    Notes = w.Notes
                                })
                                .OrderBy(w => w.Name)
                                .ToListAsync();
        }

        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Warehouse>> GetWarehouse(long id, [FromQuery(Name = "includeareas")] bool includeAreas)
        {
            WarehouseModel warehouse = null;
            if (includeAreas)
                 warehouse = await _context.Warehouses.Include("Areas").Where(w => w.Id == id).FirstOrDefaultAsync();
            else
                warehouse = await _context.Warehouses.Where(w => w.Id == id).FirstOrDefaultAsync();
            
            var dto = new Warehouse()
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Notes = warehouse.Notes
            };

            if (includeAreas)
            {
                dto.Areas = warehouse.Areas.Select(a => new Area()
                                                {
                                                    Id = a.Id,
                                                    Name = a.Name,
                                                    Description = a.Description,
                                                    Notes = a.Notes
                                                })
                                             .ToList();
            }
            return dto;
        }

        // GET: api/Warehouses/5/CurrentStock
        [HttpGet("{id}/currentstock")]
        [Authorize]
        //public async Task<ActionResult<Warehouse>> GetWarehouseCurrentStock(long id)
        //{
        //    var details = _context.Components.Include("LoadedCartDetails")
        //                                    .Include("LoadedCartDetails.LoadedCart")
        //                                    .Include("LoadedCartDetails.LoadedCart.Cart")
        //                                    .Include("LoadedCartDetails.LoadedCart.Location")
        //                                    .Include("LoadedCartDetails.LoadedCart.Location.Area")
        //                                    .SelectMany(c => c.LoadedCartDetails)
        //                                    .Where(lcd => lcd.LoadedCart.Location.Area.WarehouseId == id)
        //                                    .OrderBy(lcd => lcd.Component.Code)
        //                                    .ThenBy(lcd => lcd.LoadedCart.Cart.SerialNumber)
        //                                    .ThenBy(lcd => lcd.LoadedCart.Location.Area.Name)
        //                                    .ThenBy(lcd => lcd.LoadedCart.Location.Name)
        //                                    .Select(lcd => new
        //                                    {
        //                                        Code = lcd.Component.Code,
        //                                        Description = lcd.Component.Description,
        //                                        SerialNumber = lcd.LoadedCart.Cart.SerialNumber,
        //                                        AreaName = lcd.LoadedCart.Location.Area.Name,
        //                                        LocationName = lcd.LoadedCart.Location.Name
        //                                    }
        //                                            );

        //    var groups = details.AsEnumerable().GroupBy(r => new StockKey()
        //    {
        //        Code = r.Code,
        //        Description = r.Description
        //    },
        //                                r => new StockDetail()
        //                                {
        //                                    SerialNumber = r.SerialNumber,
        //                                    AreaName = r.AreaName,
        //                                    LocationName = r.LocationName
        //                                },
        //                                new StockKeyComparer());

        //    var response = groups.Select(g => new Stock()
        //    {
        //        ComponentCode = g.Key.Code,
        //        ComponentDescription = g.Key.Description,
        //        Details = g.Select(d => new StockDetail()
        //        {
        //            SerialNumber = d.SerialNumber,
        //            AreaName = d.AreaName,
        //            LocationName = d.LocationName
        //        })
        //    })
        //                        .ToList();

        //    return Ok(response);
        //}
        public async Task<ActionResult<IEnumerable<Stock>>> GetWarehouseCurrentStock(long id)
        {
            var details = _context.LoadedCartDetails.Include("LoadedCart")
                                                    .Include("LoadedCart.Cart")
                                                    .Include("LoadedCart.Location")
                                                    .Include("LoadedCart.Location.Area")
                                                    .Include("Component")
                                                    .Where(lcd => lcd.LoadedCart.Location.Area.WarehouseId == id)
                                                    .OrderBy(lcd => lcd.Component.Code)
                                                    .ThenBy(lcd => lcd.LoadedCart.Cart.SerialNumber)
                                                    .ThenBy(lcd => lcd.LoadedCart.Location.Area.Name)
                                                    .ThenBy(lcd => lcd.LoadedCart.Location.Name)
                                                    .Select(lcd => new {
                                                        Code = lcd.Component.Code,
                                                        Description = lcd.Component.Description,
                                                        SerialNumber = lcd.LoadedCart.Cart.SerialNumber,
                                                        AreaName = lcd.LoadedCart.Location.Area.Name,
                                                        LocationName = lcd.LoadedCart.Location.Name
                                                    });

            //NOTA BENE: essendo un modello di test su un volume di dati limitato la creazione della master details
            //viene implementata con una query group by in memory.

            //In environmente reale con volume di dati crescente è preferibile implementarla attraverso una stored procedure 
            //che restituisca un dataset multiplo mappato su master e details con counting delle righe (qta) con group by sql
            //es _context.Set<Models.BaseModel>().FromSqlRaw("exec sp_proc",params) --> con calcolo count qty grouped per componente in stored e restituzione di 
            //recordset flatten con colonne di grouing ripetute per ogni dettaglio ordinate con grouping in memory SOLO per creare struttura MD

            //alternativa 2 :
            //da details query di group by per componene per calcolo totali -> materializzazione totali -> materializzazione dettagli -> join in memory
            //tra totali e dettagli per creare contratto master/details da restituire. In questo modo si sfruttano aggregazioni su engine DB 
            //con query creata da plan cached sulla sottoquery dei dettagli
            //es  var headers = details.GroupBy((lc) => lc.ComponentId).Select((lc) => new { lc.Key, tot = lc.Count()}).ToList(); --> rows=details.ToList(); --> join tra headers e details con grouping in memory per creare master details

            //non è possibile implementare un group by master details via LINQ to SQL perchè è necessario restituire sia il risultato
            //dell'aggregazione sia i dettagli delle righe
            //es: var detailsresponse=details.ToList()-> materializzazione con plan caching -> var headers=details.Group

            var groups = details.AsEnumerable()
                                .GroupBy(r => new StockKey() {
                                            Code = r.Code,
                                            Description = r.Description
                                        }, 
                                        r => new StockDetail() {
                                            SerialNumber = r.SerialNumber,
                                            AreaName = r.AreaName,
                                            LocationName = r.LocationName
                                        }, 
                                        new StockKeyComparer()) ;

            var response = groups.Select(g => new Stock() {
                                    ComponentCode = g.Key.Code, 
                                    ComponentDescription = g.Key.Description, 
                                    ComponentQty = g.Count(), 
                                    Details = g.Select(d => new StockDetail() { 
                                        SerialNumber = d.SerialNumber, 
                                        AreaName = d.AreaName, 
                                        LocationName = d.LocationName 
                                    })
                                })
                                .ToList();

            return Ok(response);
        }

        // GET: api/Warehouses/5/CurrentStock
        [HttpGet("{id}/loadedcarts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LoadedCart>>> GetWarehouseLoadedCarts(long id)
        {
            var lcList = await _context.LoadedCarts.Include("Cart")
                                                    .Include("Location")
                                                    .Include("Location.Area")
                                                    .Where(lc => lc.Location.Area.WarehouseId == id && !lc.DateOut.HasValue)
                                                    .OrderBy(lc => lc.Year)
                                                    .ThenBy(lc => lc.Progressive)
                                                    .Select(lc => new LoadedCart() {
                                                        Id = lc.Id,
                                                        Year = lc.Year,
                                                        Progressive = lc.Progressive,
                                                        Description = lc.Description,
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
                                                            AreaId = lc.Location.AreaId,
                                                            Area = new Area() { 
                                                                Id = lc.Location.Area.Id,
                                                                Name = lc.Location.Area.Name,
                                                                Description = lc.Location.Area.Description
                                                            }
                                                        },
                                                        DateIn = lc.DateIn
                                                    })
                                                    .ToListAsync();

            return Ok(lcList);
        }

        // GET: api/Warehouses/{id}/Areas
        [HttpGet("{id}/areas")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Area>>> GetWarehouseAreas(long id)
        {
            var exists = await _context.Warehouses.Where(w => w.Id == id).AnyAsync();
            if (!exists)
            {
                return NotFound("Warehouse not found");
            }

            var areas = await _context.Areas.Where(a => a.WarehouseId == id)
                                            .Select(a => new Area() { 
                                                Id = a.Id,
                                                Name = a.Name,
                                                Description = a.Description,
                                                Notes = a.Notes
                                            })
                                            .OrderBy(a => a.Name)
                                            .ToListAsync();
            return Ok(areas);
        }

        // PUT: api/Warehouses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> PutWarehouse(long id, Warehouse dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            warehouse.Name = dto.Name;
            warehouse.Description = dto.Description;
            warehouse.Notes = dto.Notes;
            warehouse.ModifiedBy = HttpContext.User.Identity.Name;
            warehouse.ModifiedOn = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
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

        // POST: api/Warehouses
        [HttpPost]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse dto)
        {
            var warehouse = new WarehouseModel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Notes = dto.Notes
            };
            warehouse.CreatedBy = HttpContext.User.Identity.Name;
            warehouse.ModifiedBy = HttpContext.User.Identity.Name;

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            //return CreatedAtAction("GetWarehouse", new { id = warehouse.Id }, warehouse);
            return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
        }

        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<IActionResult> DeleteWarehouse(long id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Warehouses/{id}/Areas
        [HttpPost("{id}/areas")]
        [Authorize(Roles = "WarehouseManager")]
        public async Task<ActionResult<Area>> PostWarehouseArea(long id, Area dto)
        {
            var exists = await _context.Warehouses.Where(w => w.Id == id).AnyAsync();
            if (!exists)
            {
                return NotFound("Warehouse not found");
            }

            var area = new AreaModel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Notes = dto.Notes,
                WarehouseId = id
            };
            area.CreatedBy = HttpContext.User.Identity.Name;
            area.ModifiedBy = HttpContext.User.Identity.Name;

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            //The CreatedAtAction method:
            //- Returns an HTTP 201 status code if successful.HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
            //- Adds a Location header to the response.The Location header specifies the URI of the newly created component item.
            //- References the GetTodoItem action to create the Location header's URI. The C# nameof keyword is used to avoid hard-coding the action name in the CreatedAtAction call.
            return CreatedAtAction(nameof(AreasController.GetArea), new { id = area.Id }, area);
        }

        private bool WarehouseExists(long id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }

    }
}

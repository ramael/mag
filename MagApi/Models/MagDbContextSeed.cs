using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Models
{
    public class MagDbContextSeed
    {
        public static async Task SeedEssentialsAsync(MagDbContext _context)
        {
            string user = "sys";

            //Seed Warehouses
            WarehouseModel warehouse = null;
            if (!_context.Warehouses.Any())
            {
                warehouse = new WarehouseModel()
                {
                    Name = "WarehouseOne",
                    Description = "Warehouse One",
                    Notes = "Warehouse One notes",
                    CreatedBy = user,
                    ModifiedBy = user
                };
                await _context.Warehouses.AddAsync(warehouse);
                await _context.SaveChangesAsync();

                AreaModel area1 = null;
                AreaModel area2 = null;
                AreaModel area3 = null;
                if (!_context.Areas.Any())
                {
                    area1 = new AreaModel()
                    {
                        Name = "Area Stock AS1",
                        Description = "Area Stock AS1",
                        Notes = "Area Stock AS1 notes",
                        Warehouse = warehouse,
                        CreatedBy = user,
                        ModifiedBy = user
                    };

                    area2 = new AreaModel()
                    {
                        Name = "Area Stock AS2",
                        Description = "Area Stock AS2",
                        Notes = "Area Stock AS2 notes",
                        Warehouse = warehouse,
                        CreatedBy = user,
                        ModifiedBy = user
                    };

                    area3 = new AreaModel()
                    {
                        Name = "Area Stock AS3",
                        Description = "Area Stock AS3",
                        Notes = "Area Stock AS3 notes",
                        Warehouse = warehouse,
                        CreatedBy = user,
                        ModifiedBy = user
                    };
                    await _context.Areas.AddRangeAsync(new List<AreaModel>() { area1, area2, area3 });
                    await _context.SaveChangesAsync();

                    if (!_context.Locations.Any())
                    {
                        var location1 = new LocationModel()
                        {
                            Name = "Locazione MD1",
                            Description = "Locazione MD1",
                            Notes = "Locazione MD1 notes",
                            Area = area1,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location2 = new LocationModel()
                        {
                            Name = "Locazione MD2",
                            Description = "Locazione MD2",
                            Notes = "Locazione MD2 notes",
                            Area = area1,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location3 = new LocationModel()
                        {
                            Name = "Locazione MD3",
                            Description = "Locazione MD3",
                            Notes = "Locazione MD3 notes",
                            Area = area1,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location4 = new LocationModel()
                        {
                            Name = "Locazione MD4",
                            Description = "Locazione MD4",
                            Notes = "Locazione MD4 notes",
                            Area = area2,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location5 = new LocationModel()
                        {
                            Name = "Locazione MD5",
                            Description = "Locazione MD5",
                            Notes = "Locazione MD5 notes",
                            Area = area2,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location6 = new LocationModel()
                        {
                            Name = "Locazione MD6",
                            Description = "Locazione MD6",
                            Notes = "Locazione MD6 notes",
                            Area = area2,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location7 = new LocationModel()
                        {
                            Name = "Locazione MD7",
                            Description = "Locazione MD7",
                            Notes = "Locazione MD7 notes",
                            Area = area3,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location8 = new LocationModel()
                        {
                            Name = "Locazione MD8",
                            Description = "Locazione MD8",
                            Notes = "Locazione MD8 notes",
                            Area = area3,
                            CreatedBy = user,
                            ModifiedBy = user
                        };

                        var location9 = new LocationModel()
                        {
                            Name = "Locazione MD9",
                            Description = "Locazione MD9",
                            Notes = "Locazione MD9 notes",
                            Area = area3,
                            CreatedBy = user,
                            ModifiedBy = user
                        };
                        await _context.Locations.AddRangeAsync(new List<LocationModel>() { location1, location2, location3, location4, location5, location6, location7, location8, location9 });
                        await _context.SaveChangesAsync();
                    }
                }
            }

            if (!_context.Carts.Any())
            {
                List<CartModel> carts = new List<CartModel>();
                for (int i = 0; i < 33; i++)
                {
                    var cart = new CartModel()
                    {
                        SerialNumber = "CR" + i.ToString("D6"),
                        Status = CartModel.StatusEnum.Available,
                        CreatedBy = user,
                        ModifiedBy = user
                    };
                    carts.Add(cart);
                }
                await _context.Carts.AddRangeAsync(carts);
                await _context.SaveChangesAsync();
            }

            if (!_context.Components.Any()) 
            {
                var comp1 = new ComponentModel()
                {
                    Code = "BATT1000",
                    Description = "Batteria 1000",
                    Notes = "",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                var comp2 = new ComponentModel()
                {
                    Code = "BATT2000",
                    Description = "Batteria 2000",
                    Notes = "Maneggiare con cura e con l'utilizzo dei guanti",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                var comp3 = new ComponentModel()
                {
                    Code = "MTH30",
                    Description = "Montante H30",
                    Notes = "Montante utilizzato in carrelli di colore giallo e nero",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                var comp4 = new ComponentModel()
                {
                    Code = "MTH45",
                    Description = "Montante H45",
                    Notes = "",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                var comp5 = new ComponentModel()
                {
                    Code = "FR343",
                    Description = "Forca 343",
                    Notes = "Forca allungabile",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                var comp6 = new ComponentModel()
                {
                    Code = "SR3",
                    Description = "Sirena 3",
                    Notes = "Rumorosa e adatta ad ambienti aperti",
                    CreatedBy = user,
                    ModifiedBy = user
                };

                await _context.Components.AddRangeAsync(new List<ComponentModel>() { comp1, comp2, comp3, comp4, comp5, comp6 });
                await _context.SaveChangesAsync();
            }

            if (!_context.LoadedCarts.Any())
            {
                var lc1 = new LoadedCartModel()
                {
                    Year = 2017,
                    Progressive = "LC0000001",
                    LocationId = _context.Locations.Where(l => l.Name == "Locazione MD1").Select(l => l.Id).FirstOrDefault(),
                    Cart = _context.Carts.Where(c => c.SerialNumber == "CR000001").FirstOrDefault(),
                    DateIn = DateTime.Now,
                    CreatedBy = user,
                    ModifiedBy = user,
                    LoadedCartDetails = new LoadedCartDetailModel[]
                    {
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "BATT1000").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        },
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "MTH30").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        },
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "FR343").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        },
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "FR343").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        }
                    }
                };
                lc1.Cart.Status = CartModel.StatusEnum.NotAvailable;

                var lc2 = new LoadedCartModel()
                {
                    Year = 2017,
                    Progressive = "LC0000002",
                    LocationId = _context.Locations.Where(l => l.Name == "Locazione MD4").Select(l => l.Id).FirstOrDefault(),
                    Cart = _context.Carts.Where(c => c.SerialNumber == "CR000002").FirstOrDefault(),
                    DateIn = DateTime.Now,
                    CreatedBy = user,
                    ModifiedBy = user,
                    LoadedCartDetails = new LoadedCartDetailModel[]
                    {
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "BATT2000").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        },
                        new LoadedCartDetailModel()
                        {
                            ComponentId = _context.Components.Where(c => c.Code == "MTH30").Select(c => c.Id).FirstOrDefault(),
                            CreatedBy = user,
                            ModifiedBy = user
                        }
                    }
                };
                lc2.Cart.Status = CartModel.StatusEnum.NotAvailable;
                await _context.LoadedCarts.AddRangeAsync(new List<LoadedCartModel>() { lc1, lc2 });
                await _context.SaveChangesAsync();
            }

        }
    }
}

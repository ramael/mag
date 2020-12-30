using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations;

namespace MagApi.Models
{
    public class MagDbContext : DbContext
    {
        public MagDbContext(DbContextOptions<MagDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.BuildIndexesFromAnnotationsForSqlServer();

            /*
             * Warehouse
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<Warehouse>()
            //            .HasIndex(w => w.Name)
            //            .IsUnique();

            /*
             * Area
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<Area>()
            //            .HasIndex(a => a.Name)
            //            .IsUnique();

            //Convention: index created on foreign key
            //modelBuilder.Entity<Area>()
            //            .HasIndex(a => a.WarehouseId);

            //Convention: fully defined non ambiguos relationships between principal and dependant property
            //            The dependant property named <principal>Id will be configured as foreign key
            //            The default delete behaviour is Cascade on required properties and ClientSetNull on optional properties
            //modelBuilder.Entity<Area>()
            //            .HasOne(a => a.Warehouse)
            //            .WithMany(w => w.Areas)
            //            .HasForeignKey(a => a.WarehouseId)
            //            .HasPrincipalKey(w => w.Id)
            //            .OnDelete(DeleteBehavior.NoAction);

            /*
             * Location
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<Location>()
            //            .HasIndex(l => l.Name)
            //            .IsUnique();

            //Convention: index created on foreign key
            //modelBuilder.Entity<Location>()
            //            .HasIndex(l => l.AreaId);

            //Convention: fully defined non ambiguos relationships between principal and dependant property
            //            The dependant property named <principal>Id will be configured as foreign key
            //            The default delete behaviour is Cascade on required properties and ClientSetNull on optional properties
            //modelBuilder.Entity<Location>()
            //            .HasOne(l => l.Area)
            //            .WithMany(a => a.Locations)
            //            .HasForeignKey(l => l.AreaId)
            //            .HasPrincipalKey(a => a.Id)
            //            .OnDelete(DeleteBehavior.NoAction);

            /*
             * Cart
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<Cart>()
            //            .HasIndex(c => c.SerialNumber)
            //            .IsUnique();

            /*
             * Compoment
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<Component>()
            //            .HasIndex(c => c.Code)
            //            .IsUnique();

            /*
             * LoadedCart
             */
            //Convention: index created on foreign key
            //modelBuilder.Entity<LoadedCart>()
            //            .HasIndex(lc => lc.LocationId);

            //Convention: index created on foreign key
            //modelBuilder.Entity<LoadedCart>()
            //            .HasIndex(lc => lc.CartId);

            // Using [IndexColumn] attribute
            //modelBuilder.Entity<LoadedCart>()
            //            .HasIndex(lc => new { lc.LocationId, lc.CartId, lc.DateIn })
            //            .IsUnique();

            //Convention: fully defined non ambiguos relationships between principal and dependant property
            //            The dependant property named <principal>Id will be configured as foreign key
            //            The default delete behaviour is Cascade on required properties and ClientSetNull on optional properties
            //modelBuilder.Entity<LoadedCart>()
            //            .HasOne(lc => lc.Location)
            //            .WithMany(l => l.LoadedCarts)
            //            .HasForeignKey(lc => lc.LocationId)
            //            .HasPrincipalKey(l => l.Id)
            //            .OnDelete(DeleteBehavior.NoAction);

            //Convention: fully defined non ambiguos relationships between principal and dependant property
            //            The dependant property named <principal>Id will be configured as foreign key
            //            The default delete behaviour is Cascade on required properties and ClientSetNull on optional properties
            //modelBuilder.Entity<LoadedCart>()
            //            .HasOne(lc => lc.Cart)
            //            .WithMany()
            //            .HasForeignKey(lc => lc.CartId)
            //            .HasPrincipalKey(l => l.Id)
            //            .OnDelete(DeleteBehavior.NoAction);

            /*
             * LoadedCartDetail
             */
            // Using [IndexColumn] attribute
            //modelBuilder.Entity<LoadedCartDetail>()
            //            .HasKey(lcd => new { lcd.LoadedCartId, lcd.ComponentId });

            //Property is part of PK, so it needs to be manually configured as FK
            modelBuilder.Entity<LoadedCartDetailModel>()
                        .HasOne(lcd => lcd.LoadedCart)
                        .WithMany(lc => lc.LoadedCartDetails)
                        .HasForeignKey(lcd => lcd.LoadedCartId)
                        .HasPrincipalKey(lc => lc.Id)
                        .OnDelete(DeleteBehavior.Cascade);

            //Property is part of PK, so it needs to be manually configured as FK
            modelBuilder.Entity<LoadedCartDetailModel>()
                        .HasOne(lcd => lcd.Component)
                        .WithMany()
                        .HasForeignKey(lcd => lcd.ComponentId)
                        .HasPrincipalKey(c => c.Id)
                        .OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<WarehouseModel> Warehouses { get; set; }

        public DbSet<AreaModel> Areas { get; set; }

        public DbSet<LocationModel> Locations { get; set; }

        public DbSet<CartModel> Carts { get; set; }

        public DbSet<ComponentModel> Components { get; set; }

        public DbSet<LoadedCartModel> LoadedCarts { get; set; }

        public DbSet<LoadedCartDetailModel> LoadedCartDetails { get; set; }
    }
}

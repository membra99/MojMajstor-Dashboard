using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Context
{
    public partial class MainContext : DbContext
    {
        #region MainDataDataSET

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttributes> ProductAttributes { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleType> SaleTypes { get; set; }
        public DbSet<Seo> Seos { get; set; }
        public DbSet<Declaration> Declarations { get; set; }


        #endregion

        private void UniversalModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.ProductId);

                entity.HasOne(x => x.Categories)
                .WithMany(x => x.Products)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Seo)
                .WithMany(x => x.Products)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Declaration)
                .WithMany(x => x.Products)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductAttributes>(entity =>
            {
                entity.HasKey(x => x.ProductAttributeId);

                entity.HasOne(x => x.Categories)
                .WithMany(x => x.ProductAttributes)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Product)
                .WithMany(x => x.ProductAttributes)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(x => x.CategoryId);
            });
        }
    }
}

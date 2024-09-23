using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QLBQA.Models
{
    public partial class QLBQA_DB : DbContext
    {

        public QLBQA_DB()
            : base("name=QLBQA_DB")
        {
            this.Database.Log = Console.Write;
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TransactStatus> TransactStatus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Salt)
                .IsFixedLength();

            modelBuilder.Entity<Color>()
                .Property(e => e.ColorCode)
                .IsFixedLength();

            modelBuilder.Entity<Color>()
                .HasMany(e => e.ProductDetails)
                .WithRequired(e => e.Color)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Email)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Salt)
                .IsFixedLength();

            modelBuilder.Entity<Image>()
                .Property(e => e.Path)
                .IsFixedLength();

            modelBuilder.Entity<Image>()
                .Property(e => e.Description)
                .IsFixedLength();

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Images)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Shipper>()
                .Property(e => e.Phone)
                .IsFixedLength();

            modelBuilder.Entity<Size>()
                .Property(e => e.SizeName)
                .IsFixedLength();

            modelBuilder.Entity<Size>()
                .HasMany(e => e.ProductDetails)
                .WithRequired(e => e.Size)
                .WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<QLBQA.Models.RegisterModel> RegisterModels { get; set; }

        public System.Data.Entity.DbSet<QLBQA.Models.LoginModel> LoginModels { get; set; }
    }
}

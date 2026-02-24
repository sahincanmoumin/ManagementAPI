using Entity.Enums;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; } //*****
        public DbSet<UserRole> UserRoles { get; set; }//******


        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // id kullanmadan yapma şekli ikisini de primary key sanıyormuş
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            base.OnModelCreating(modelBuilder);

            // Enumı stringe çevirm
            modelBuilder.Entity<Animal>()
                .Property(a => a.Type)
                .HasConversion<string>();
                
            // Ürünler string
            modelBuilder.Entity<Product>()
                .Property(p => p.Name) 
                .HasConversion<string>();
        }
    }
}
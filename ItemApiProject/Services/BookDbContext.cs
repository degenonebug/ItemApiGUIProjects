using ItemApiProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemApiProject.Services
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
        {
            //Database.Migrate();
        }

        public virtual DbSet<Book> Books { get; set; }


        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<BookCategory> BookCategories { get; set; }
        
        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookCategory>()
                        .HasKey(bc => new { bc.BookId, bc.CategoryId });
            modelBuilder.Entity<BookCategory>()
                        .HasOne(b => b.Book)
                        .WithMany(bc => bc.BookCategories)
                        .HasForeignKey(b => b.BookId);
            modelBuilder.Entity<BookCategory>()
                        .HasOne(c => c.Category)
                        .WithMany(bc => bc.BookCategories)
                        .HasForeignKey(c => c.CategoryId);

           
        }
    }
}

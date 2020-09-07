using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItemApiProject.Models;

namespace ItemApiProject.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookDbContext;

        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool BookExists(int bookId)
        {
            return _bookDbContext.Books.Any(b => b.Id == bookId);
        }

        

        public bool CreateBook(List<int> categoriesId, Book book)
        {
           
            var categories = _bookDbContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            _bookDbContext.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _bookDbContext.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId)
        {
            return _bookDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        

        

        public ICollection<Book> GetBooks()
        {
            return _bookDbContext.Books.OrderBy(b => b.Title).ToList();
        }

        
        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateBook(List<int> categoriesId, Book book)
        {
            
            var categories = _bookDbContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            
            var bookCategoriesToDelete = _bookDbContext.BookCategories.Where(b => b.BookId == book.Id);

            
            _bookDbContext.RemoveRange(bookCategoriesToDelete);

            

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            _bookDbContext.Update(book);

            return Save();
        }
    }
}

using ItemApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemApiProject.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        
       
        bool BookExists(int bookId);
        
       

        bool CreateBook(List<int> categoriesId, Book book);
        bool UpdateBook(List<int> categoriesId, Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}

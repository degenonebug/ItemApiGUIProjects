using ItemApiProject.Dtos;
using ItemApiProject.Models;
using ItemGUI.Components;
using ItemGUI.Services;
using ItemGUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ItemGUI.Controllers
{
    public class HomeController : Controller
    {
        IBookRepositoryGUI _bookRepository;
        ICategoryRepositoryGUI _categoryRepository;


        public HomeController(IBookRepositoryGUI bookRepository, 
                              ICategoryRepositoryGUI categoryRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;

        }

        public IActionResult Index()
        {
            var books = _bookRepository.GetBooks();

            if(books.Count() <= 0)
            {
                ViewBag.Message = "Ошибка при получении товаров из базы данных или товар не существует";
            }

            var bookAuthorsCategoriesRatingViewModel = new List<BookCategoriesViewModel>();

            foreach(var book in books)
            {



                var categories = _categoryRepository.GetAllCategoriesForBook(book.Id).ToList();
                if (categories.Count() <= 0)
                    ModelState.AddModelError("", "Ошибка при получении категории");



                bookAuthorsCategoriesRatingViewModel.Add(new BookCategoriesViewModel
                {
                    Book = book,
                    Categories = categories,

                });
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(bookAuthorsCategoriesRatingViewModel);
        }

        public IActionResult GetBookById(int bookId)
        {
            var completeBookViewModel = new CompleteBookViewModel
            {

            };

            var book = _bookRepository.GetBookById(bookId);

            if (book == null)
            {
                ModelState.AddModelError("", "Ошибка при получении товара");
                book = new BookDto();
            }

            var categories = _categoryRepository.GetAllCategoriesForBook(bookId);
            if (categories.Count() <= 0)
            {
                ModelState.AddModelError("", "Ошибка при получении категории");
            }


            completeBookViewModel.Book = book;
            completeBookViewModel.Categories = categories;




            if (!ModelState.IsValid)
            {
                ViewBag.BookMessage = "Ошибка при получении досье товара";
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(completeBookViewModel);
        }

        [HttpGet]
        public IActionResult CreateBook()
        {

            var categories = _categoryRepository.GetCategories();

            if (categories.Count() <= 0)
            {
                ModelState.AddModelError("", "Ошибка при получении категории");
            }

            var categoryList = new CategoriesList(categories.ToList());

            var createUpdateBook = new CreateUpdateBookViewModel
            {

                CategorySelectListItems = categoryList.GetCategoriesList()
            };

            return View(createUpdateBook);

        }

        [HttpPost]
        public IActionResult CreateBook(IEnumerable<int> CategoryIds, 
            CreateUpdateBookViewModel bookToCreate)
        {
            using (var client = new HttpClient())
            {
                var book = new Book()
                {
                    
                    Id = bookToCreate.Book.Id,
                    Title = bookToCreate.Book.Title,
                    Description = bookToCreate.Book.Description,
                    Price = bookToCreate.Book.Price,
                    Color = bookToCreate.Book.Color,
                    Weight = bookToCreate.Book.Weight,
                    Size = bookToCreate.Book.Size

                    //books?catId=?
                };

                var uriParameters = GetAuthorsCategoriesUri(CategoryIds.ToList());

                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.PostAsJsonAsync($"books?{uriParameters}", book);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTaskNewBook = result.Content.ReadAsAsync<Book>();
                    readTaskNewBook.Wait();

                    var newBook = readTaskNewBook.Result;

                    TempData["SuccessMessage"] = $"Товар {book.Title} был успешно добавлен";
                    return RedirectToAction("GetBookById", new { bookId = newBook.Id });
                }

                if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Ошибка. Товар не был добавлен");
                }
                              
            }

            
            var categoryList = new CategoriesList(_categoryRepository.GetCategories().ToList());
            
            bookToCreate.CategorySelectListItems = categoryList.GetCategoriesList(CategoryIds.ToList());
            bookToCreate.CategoryIds = CategoryIds.ToList();

            return View(bookToCreate);
        }

        [HttpGet]
        public IActionResult UpdateBook(int bookId)
        {
            var bookDto = _bookRepository.GetBookById(bookId);
            var categoryList = new CategoriesList(_categoryRepository.GetCategories().ToList());

            var bookViewModel = new CreateUpdateBookViewModel
            {
                Book = bookDto,
                CategorySelectListItems = categoryList.GetCategoriesList(_categoryRepository.GetAllCategoriesForBook(bookId)
                                        .Select(c => c.Id).ToList())
            };

            return View(bookViewModel);
        }

        [HttpPost]
        public IActionResult UpdateBook(IEnumerable<int>CategoryIds, 
            CreateUpdateBookViewModel bookToUpdate)
        {
            using(var client = new HttpClient())
            {
                var book = new Book()
                {
                    Id = bookToUpdate.Book.Id,
                    Title = bookToUpdate.Book.Title,
                    Description = bookToUpdate.Book.Description,
                    Price = bookToUpdate.Book.Price,
                    Color = bookToUpdate.Book.Color,
                    Weight = bookToUpdate.Book.Weight,
                    Size = bookToUpdate.Book.Size
                };

                var uriParameters = GetAuthorsCategoriesUri(CategoryIds.ToList());

                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.PutAsJsonAsync($"books/{book.Id}?{uriParameters}", book);
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Товар {book.Title} был успешно обновлен";
                    return RedirectToAction("GetBookById", new { bookId = book.Id });
                }

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Ошибка. Товар не был добавлен");
                }
                
            }

            
            var categoryList = new CategoriesList(_categoryRepository.GetCategories().ToList());
            
            bookToUpdate.CategorySelectListItems = categoryList.GetCategoriesList(CategoryIds.ToList());
            bookToUpdate.CategoryIds = CategoryIds.ToList();

            return View(bookToUpdate);
        }

        [HttpGet]
        public IActionResult DeleteBook(int bookId)
        {
            var bookDto = _bookRepository.GetBookById(bookId);

            return View(bookDto);
        }

        [HttpPost]
        public IActionResult DeleteBook(int bookId, string bookTitle)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:60039/api/");
                var responseTask = client.DeleteAsync($"books/{bookId}");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Товар {bookTitle} был успешно удален";

                    return RedirectToAction("Index");
                }
                
                ModelState.AddModelError("", "Ошибка. Товар не был удален");
            }

            var bookDto = _bookRepository.GetBookById(bookId);
            return View(bookDto);
        }

        private string GetAuthorsCategoriesUri(List<int> categoryIds)
        {
            var uri = "";
            

            foreach (var categoryId in categoryIds)
            {
                uri += $"catId={categoryId}&";
            }

            return uri;
        }
    }
}

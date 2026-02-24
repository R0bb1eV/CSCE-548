using BookTracker.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace BookTracker.Business.Implementations
{
    public class BookBusiness : IBookBusiness
    {
        private readonly DataProvider _dataProvider;

        public BookBusiness(DataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public void AddBook(Book book)
        {
            _dataProvider.CreateBook(book);
        }

        public List<Book> GetAllBooks()
        {
            return _dataProvider.ReadAllBooks();
        }

        public Book? GetBookById(int id)
        {
            return _dataProvider.ReadBookById(id);
        }

        public void UpdateBook(Book book)
        {
            _dataProvider.UpdateBook(book);
        }

        public void RemoveBook(int id)
        {
            _dataProvider.DeleteBook(id);
        }
    }
}

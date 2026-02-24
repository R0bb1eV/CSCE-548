using System.Collections.Generic;

namespace BookTracker.Business.Interfaces
{
    public interface IBookBusiness
    {
        void AddBook(Book book);
        List<Book> GetAllBooks();
        Book? GetBookById(int id);
        void UpdateBook(Book book);
        void RemoveBook(int id);
    }
}

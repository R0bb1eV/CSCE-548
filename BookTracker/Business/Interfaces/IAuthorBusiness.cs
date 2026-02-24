using System.Collections.Generic;

namespace BookTracker.Business.Interfaces
{
    public interface IAuthorBusiness
    {
        void AddAuthor(Author author);
        List<Author> GetAllAuthors();
        Author? GetAuthorById(int id);
        void UpdateAuthor(Author author);
        void RemoveAuthor(int id);
    }
}

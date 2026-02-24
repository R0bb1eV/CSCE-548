using BookTracker.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace BookTracker.Business.Implementations
{
    public class AuthorBusiness : IAuthorBusiness
    {
        private readonly DataProvider _dataProvider;

        public AuthorBusiness(DataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public void AddAuthor(Author author)
        {
            _dataProvider.CreateAuthor(author);
        }

        public List<Author> GetAllAuthors()
        {
            return _dataProvider.ReadAllAuthors();
        }

        public Author? GetAuthorById(int id)
        {
            return _dataProvider.ReadAuthorById(id);
        }

        public void UpdateAuthor(Author author)
        {
            _dataProvider.UpdateAuthor(author);
        }

        public void RemoveAuthor(int id)
        {
            _dataProvider.DeleteAuthor(id);
        }
    }
}

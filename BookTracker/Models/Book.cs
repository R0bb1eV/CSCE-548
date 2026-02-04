using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTracker
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public string Genre { get; set; }
        public string PublishingHouse { get; set; }
        public int YearOfRelease { get; set; }
        public string ISBN { get; set; }
        public int AuthorId { get; set; }

        public override string ToString()
        {
            return $"BookId: {ID}, Title: {Title}, AuthorId: {AuthorId}, Genre: {Genre}, Pages: {PageCount}, Year: {YearOfRelease}, ISBN: {ISBN}";
        }
    }
}
using System;
using System.Collections.Generic;
using BookTracker;

namespace BookTrackerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // MySQL connection string
            string connectionString = "server=localhost;database=booktracker;user=root;password=password;SslMode=Preferred";
            var dp = new DataProvider(connectionString);

            Console.WriteLine("===== TEST HARNESS START =====\n");

            // =================== AUTHOR TEST ===================
            Console.WriteLine("----- AUTHOR TABLE TEST -----");

            // Insert
            var newAuthor = new Author { FirstName = "Test", MiddleName = "T", LastName = "Author", BirthYear = 1990 };
            dp.CreateAuthor(newAuthor);

            Console.WriteLine("Inserted Author:");
            var authors = dp.ReadAllAuthors();
            PrintList(authors);

            // Update
            var authorToUpdate = authors[^1]; // Last author (the one we just added)
            authorToUpdate.FirstName = "Updated";
            dp.UpdateAuthor(authorToUpdate);

            Console.WriteLine("Updated Author:");
            PrintList(dp.ReadAllAuthors());

            // Delete
            dp.DeleteAuthor(authorToUpdate.AuthorId);
            Console.WriteLine("Deleted Author:");
            PrintList(dp.ReadAllAuthors());

            // =================== BOOK TEST ===================
            Console.WriteLine("\n----- BOOK TABLE TEST -----");

            // Insert
            var newBook = new Book
            {
                Title = "Test Book",
                PageCount = 123,
                Genre = "Fantasy",
                PublishingHouse = "Test House",
                YearOfRelease = 2026,
                ISBN = "1234567890123",
                AuthorId = 1 // Make sure author 1 exists
            };
            dp.CreateBook(newBook);

            Console.WriteLine("Inserted Book:");
            PrintList(dp.ReadAllBooks());

            // Update
            var bookToUpdate = dp.ReadAllBooks()[^1];
            bookToUpdate.Title = "Updated Book";
            dp.UpdateBook(bookToUpdate);

            Console.WriteLine("Updated Book:");
            PrintList(dp.ReadAllBooks());

            // Delete
            dp.DeleteBook(bookToUpdate.ID);
            Console.WriteLine("Deleted Book:");
            PrintList(dp.ReadAllBooks());

            // =================== USER TEST ===================
            Console.WriteLine("\n----- USER TABLE TEST -----");

            // Insert
            var newUser = new User { Username = "testuser", Email = "testuser@example.com", DOB = new DateTime(2000, 1, 1) };
            dp.CreateUser(newUser);

            Console.WriteLine("Inserted User:");
            PrintList(dp.ReadAllUsers());

            // Update
            var userToUpdate = dp.ReadAllUsers()[^1];
            userToUpdate.Username = "updateduser";
            dp.UpdateUser(userToUpdate);

            Console.WriteLine("Updated User:");
            PrintList(dp.ReadAllUsers());

            // Delete
            dp.DeleteUser(userToUpdate.UserId);
            Console.WriteLine("Deleted User:");
            PrintList(dp.ReadAllUsers());

            // =================== ACTIVITY TEST ===================
            Console.WriteLine("\n----- ACTIVITY TABLE TEST -----");

            // Insert
            var newActivity = new Activity
            {
                UserId = 1, // Make sure user 1 exists
                BookId = 1, // Make sure book 1 exists
                BookStatus = "reading",
                ProgressCompleted = 25,
                StartDate = DateTime.Today,
                EndDate = null
            };
            dp.CreateActivity(newActivity);

            Console.WriteLine("Inserted Activity:");
            PrintList(dp.ReadAllActivities());

            // Update
            var activityToUpdate = dp.ReadAllActivities()[^1];
            activityToUpdate.BookStatus = "completed";
            activityToUpdate.ProgressCompleted = 100;
            activityToUpdate.EndDate = DateTime.Today;
            dp.UpdateActivity(activityToUpdate);

            Console.WriteLine("Updated Activity:");
            PrintList(dp.ReadAllActivities());

            // Delete
            dp.DeleteActivity(activityToUpdate.ActivityId);
            Console.WriteLine("Deleted Activity:");
            PrintList(dp.ReadAllActivities());

            Console.WriteLine("\n===== TEST HARNESS END =====");
        }

        static void PrintList<T>(List<T> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }
    }
}

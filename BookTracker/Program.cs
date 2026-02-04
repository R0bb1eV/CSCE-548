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

            // Seed if empty
            if (dp.ReadAllAuthors().Count == 0)
            {
                dp.CreateAuthor(new Author { FirstName = "J.K.", LastName = "Rowling", BirthYear = 1965 });
                dp.CreateAuthor(new Author { FirstName = "George", LastName = "Orwell", BirthYear = 1903 });
            }

            // Insert
            var newAuthor = new Author { FirstName = "Test", MiddleName = "T", LastName = "Author", BirthYear = 1990 };
            dp.CreateAuthor(newAuthor);
            Console.WriteLine("Inserted Author:");
            PrintList(dp.ReadAllAuthors());

            // Update
            var authorToUpdate = dp.ReadAllAuthors()[^1]; // Last author (the one we just added)
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

            // Seed if empty
            if (dp.ReadAllBooks().Count == 0)
            {
                var firstAuthor = dp.ReadAllAuthors()[0];
                dp.CreateBook(new Book
                {
                    Title = "Harry Potter and the Sorcerer's Stone",
                    PageCount = 309,
                    Genre = "Fantasy",
                    PublishingHouse = "Bloomsbury",
                    YearOfRelease = 1997,
                    ISBN = "9780747532699",
                    AuthorId = firstAuthor.AuthorId
                });
            }

            // Insert
            var newBook = new Book
            {
                Title = "Test Book",
                PageCount = 123,
                Genre = "Fantasy",
                PublishingHouse = "Test House",
                YearOfRelease = 2026,
                ISBN = "1234567890123",
                AuthorId = dp.ReadAllAuthors()[0].AuthorId
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

            // Seed if empty
            if (dp.ReadAllUsers().Count == 0)
            {
                dp.CreateUser(new User
                {
                    Username = "user1",
                    Email = "user1@example.com",
                    DOB = new DateTime(1990, 1, 1),
                    AccountCreationDate = DateTime.Now
                });
            }

            // Insert
            var newUser = new User
            {
                Username = "testuser",
                Email = "testuser@example.com",
                DOB = new DateTime(2000, 1, 1)
            };
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

            // Seed if empty
            if (dp.ReadAllActivities().Count == 0)
            {
                var firstUser = dp.ReadAllUsers()[0];
                var firstBook = dp.ReadAllBooks()[0];
                dp.CreateActivity(new Activity
                {
                    UserId = firstUser.UserId,
                    BookId = firstBook.ID,
                    BookStatus = "reading",
                    ProgressCompleted = 25,
                    StartDate = DateTime.Today,
                    EndDate = null
                });
            }

            // Insert
            var newActivity = new Activity
            {
                UserId = dp.ReadAllUsers()[0].UserId,
                BookId = dp.ReadAllBooks()[0].ID,
                BookStatus = "reading",
                ProgressCompleted = 25,
                StartDate = DateTime.Today
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
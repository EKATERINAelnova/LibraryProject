using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryProject.Service;
using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryProject.Service.Tests
{
    [TestClass]
    public class BookServiceTests
    {
        private LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        //GetBooks

        [TestMethod]
        public async Task GetBooks_PagedBooks()
        {
            var db = CreateContext();

            db.BooksInfo.AddRange(
                new BookInfo { BookId = 1, Title = "A" },
                new BookInfo { BookId = 2, Title = "B" },
                new BookInfo { BookId = 3, Title = "C" }
            );
            await db.SaveChangesAsync();

            var service = new BookService(db);

            var result = await service.GetBooks(1, 2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.First().BookId);
        }

        //GetTotalBooksCount

        [TestMethod]
        public async Task GetTotalBooksCount_Correct()
        {
            var db = CreateContext();

            db.BooksInfo.AddRange(
                new BookInfo { BookId = 1 },
                new BookInfo { BookId = 2 }
            );
            await db.SaveChangesAsync();

            var service = new BookService(db);

            var count = await service.GetTotalBooksCount();

            Assert.AreEqual(2, count);
        }

        //GetAllBooksAsync
        [TestMethod]
        public async Task GetAllBooksOrderedByTitle()
        {
            var db = CreateContext();

            db.Books.AddRange(
                new Books { BookId = 1, Title = "Zoo" },
                new Books { BookId = 2, Title = "Alpha" }
            );
            await db.SaveChangesAsync();

            var service = new BookService(db);

            var result = await service.GetAllBooksAsync();

            Assert.AreEqual("Alpha", result.First().Title);
        }

        //Словари
        [TestMethod]
        public async Task GetAllAuthorsDoesNotThrow()
        {
            var service = new BookService(CreateContext());
            var result = await service.GetAllAuthorsAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllPublishersDoesNotThrow()
        {
            var service = new BookService(CreateContext());
            var result = await service.GetAllPublishersAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllSectionsDoesNotThrow()
        {
            var service = new BookService(CreateContext());
            var result = await service.GetAllSectionsAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllCopyStatusesDoesNotThrow()
        {
            var service = new BookService(CreateContext());
            var result = await service.GetAllCopyStatusesAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetCopiesInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.GetCopiesAsync(1, 1, 10, false));
        }

        [TestMethod]
        public async Task ReserveCopyInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.ReserveCopyAsync(1, 1));
        }

        [TestMethod]
        public async Task CancelReservationInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CancelReservationAsync(1));
        }

        [TestMethod]
        public async Task AddCopyInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.AddCopyAsync(1, "Pub", "Sec", 2023, 1, 1, 1, "OK", 100, 1));
        }

        [TestMethod]
        public async Task UpdateCopyInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateCopyAsync(1, "Pub", "Sec", 2023, 1, 1, 1, "OK", 1));
        }

        [TestMethod]
        public async Task DeleteCopyInvalidOperation()
        {
            var service = new BookService(CreateContext());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.DeleteCopyAsync(1, 1));
        }
    }
}

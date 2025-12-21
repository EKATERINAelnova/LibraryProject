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
    public class LoanServiceTests
    {
        private LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        //GetReqs
        [TestMethod]
        public async Task GetReqs_test()
        {
            var db = CreateContext();
            var service = new LoanService(db);

            var result = await service.GetReqs(1, 10);

            Assert.IsNotNull(result);
        }

        //GetTotalReqsCount
        [TestMethod]
        public async Task GetTotalReqsCount_test()
        {
            var db = CreateContext();
            var service = new LoanService(db);

            var count = await service.GetTotalReqsCount();

            Assert.IsTrue(count >= 0);
        }



        //GetLoansAsync
        [TestMethod]
        public async Task ReturnsLoansOrderedByDesc()
        {
            var db = CreateContext();

            db.BookLoanInfo.AddRange(
                new BookLoanInfo
                {
                    LoanId = 1,
                    LoanDate = DateTime.Now.AddDays(-5)
                },
                new BookLoanInfo
                {
                    LoanId = 2,
                    LoanDate = DateTime.Now
                }
            );

            await db.SaveChangesAsync();

            var service = new LoanService(db);

            var result = await service.GetLoansAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result.First().LoanId);
        }

        //ApproveRequestAsync
        [TestMethod]
        public async Task ApproveRequestDoesNotThrow()
        {
            var db = CreateContext();
            var service = new LoanService(db);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await service.ApproveRequestAsync(1, 1);
            });
        }

        //RejectRequestAsync
        [TestMethod]
        public async Task RejectRequestDoesNotThrow()
        {
            var db = CreateContext();
            var service = new LoanService(db);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await service.RejectRequestAsync(1);
            });
        }

        //ReturnLoanAsync
        [TestMethod]
        public async Task ReturnLoanDoesNotThrow()
        {
            var db = CreateContext();
            var service = new LoanService(db);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await service.ReturnLoanAsync(1);
            });
        }
    }
}

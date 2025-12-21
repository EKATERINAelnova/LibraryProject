using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryProject.Service;
using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LibraryProject.Service.Tests
{
    [TestClass]
    public class WorkerServiceTests
    {
        private LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        //GetWorkerForEdit
        [TestMethod]
        public async Task GetWorkerForEditCorrect()
        {
            var db = CreateContext();

            var user = new Users
            {
                UserId = 1,
                Login = "worker1",
                Email = "worker@test.com",
                Role = "worker",
                Worker = new Workers
                {
                    UserId = 1,
                    FullName = "Иван Иванов",
                    PhoneNumber = "+79995556677",
                    Address = "Адрес",
                    Inn = "1234567890",
                    Passport = "12345678901",
                    Gender = "М",
                    Position = "Библиотекарь",
                    Salary = 50000
                }
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = new WorkerService(db);

            var result = await service.GetWorkerForEdit(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("worker1", result.Login);
            Assert.AreEqual("Иван Иванов", result.FullName);
            Assert.AreEqual("+79995556677", result.Phone);
            Assert.AreEqual("1234567890", result.Inn);
        }

        //DeleteUserAsync
        [TestMethod]
        public async Task DeleteUserInvalidOperation()
        {
            var db = CreateContext();
            var service = new WorkerService(db);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.DeleteUserAsync(1, 1));
        }

        //UpdateWorkerAsync
        [TestMethod]
        public async Task UpdateWorkerInvalidOperation()
        {
            var db = CreateContext();
            var service = new WorkerService(db);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateWorkerAsync(1, 1, "login", "email@test.com",
                    "address", "+79995556677", "1234567890", "12345678901",
                    "ФИО", "М", "Должность", 50000));
        }
    }
}

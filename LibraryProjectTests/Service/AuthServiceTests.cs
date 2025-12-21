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
    public class AuthServiceTests
    {
        private LibraryContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LibraryContext(options);
        }

        //Регистрация
        [TestMethod]
        public async Task Register_InvalidEmail()
        {
            var context = CreateInMemoryContext();
            var service = new AuthService(context);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.Register(
                    login: "user1",
                    email: "invalid-email",
                    password: "Password123",
                    address: "Москва, ул. Ленина",
                    phoneNumber: "+79991234567"
                ));
        }

        [TestMethod]
        public async Task Register_InvalidPhone()
        {
            var context = CreateInMemoryContext();
            var service = new AuthService(context);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.Register(
                    login: "user1",
                    email: "test@mail.com",
                    password: "Password123",
                    address: "Москва, ул. Ленина",
                    phoneNumber: "12345"
                ));
        }

        [TestMethod]
        public async Task Register_WeakPassword()
        {
            var context = CreateInMemoryContext();
            var service = new AuthService(context);

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.Register(
                    login: "user1",
                    email: "test@mail.com",
                    password: "12345",
                    address: "Москва, ул. Ленина",
                    phoneNumber: "+79991234567"
                ));
        }

        //Вход
        [TestMethod]
        public void Login_ReturnsTrue()
        {
            var context = CreateInMemoryContext();

            var user = new Users
            {
                Login = "user1",
                Email = "test@mail.com",
                PasswordHash = AuthService.HashPassword("Password123"),
                Role = "reader"
            };

            context.Users.Add(user);
            context.SaveChanges();

            var service = new AuthService(context);

            var result = service.Login("test@mail.com", "Password123");

            Assert.IsTrue(result);
            Assert.IsNotNull(service.CurrentUser);
            Assert.AreEqual("user1", service.CurrentUser.Login);
        }

        [TestMethod]
        public void Login_ReturnsFalse()
        {
            var context = CreateInMemoryContext();

            context.Users.Add(new Users
            {
                Login = "user1",
                Email = "test@mail.com",
                PasswordHash = AuthService.HashPassword("Password123"),
                Role = "reader"
            });

            context.SaveChanges();

            var service = new AuthService(context);

            var result = service.Login("test@mail.com", "WrongPassword");

            Assert.IsFalse(result);
            Assert.IsNull(service.CurrentUser);
        }

        //Хэширование пароля
        [TestMethod]
        public void HashPassword_ReturnsSameHash()
        {
            string password = "Password123";

            string hash1 = AuthService.HashPassword(password);
            string hash2 = AuthService.HashPassword(password);

            Assert.AreEqual(hash1, hash2);
            Assert.AreNotEqual(password, hash1);
        }

        // INN валидация
        [TestMethod]
        public void ValidateInn_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                AuthService.ValidateInn("12345"));
        }

        [TestMethod]
        public void ValidateInn_DoesNotThrow()
        {
            AuthService.ValidateInn("1234567890");
        }

        //Выход
        [TestMethod]
        public void ClearsCurrentUser()
        {
            var context = CreateInMemoryContext();

            var user = new Users
            {
                Login = "user1",
                Email = "test@mail.com",
                PasswordHash = AuthService.HashPassword("Password123"),
                Role = "reader"
            };

            context.Users.Add(user);
            context.SaveChanges();

            var service = new AuthService(context);
            service.Login("test@mail.com", "Password123");

            service.Logout();

            Assert.IsNull(service.CurrentUser);
        }
    }
}

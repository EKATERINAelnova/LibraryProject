using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryProject.Service
{
    public class AuthService
    {
        private readonly LibraryContext _db;

        public AuthService(LibraryContext db)
        {
            _db = db;
        }

        public Users? CurrentUser { get; private set; }

        public bool Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public async Task Register(string login, string email, string password, string address, string phoneNumber)
        {
            // Проверки валидности
            if (!IsValidEmail(email))
                throw new ArgumentException("Email введён некорректно.");

            if (!IsValidPhone(phoneNumber))
                throw new ArgumentException("Телефон введён некорректно. Используйте формат: +79991234567 или 89991234567.");

            if (!IsValidPassword(password))
                throw new ArgumentException("Пароль должен быть минимум 8 символов и содержать хотя бы одну букву и одну цифру.");

            if (string.IsNullOrWhiteSpace(address) || address.Length < 5)
                throw new ArgumentException("Адрес введён некорректно. Он должен быть не пустым и содержать минимум 5 символов.");

            string passwordHash = HashPassword(password);

            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call create_reader({login}, {email}, {passwordHash}, {address}, {phoneNumber})");
        }

        // Проверка email через Regex
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        // Проверка телефона через Regex
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string pattern = @"^\+?\d{10,15}$"; // допускается + в начале, 10-15 цифр
            return Regex.IsMatch(phone, pattern);
        }

        // Проверка пароля
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasLetter = Regex.IsMatch(password, @"[a-zA-Z]");
            bool hasDigit = Regex.IsMatch(password, @"\d");

            return hasLetter && hasDigit;
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }
        public async Task RegisterWorker(string login, string email, string password, string address, string phone, string inn, string passport, string fullName, string gender, string position, decimal salary)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Обязательные поля не могут быть пустыми");
            }

            // Проверка email
            if (!System.Text.RegularExpressions.Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email указан неверно");

            // Проверка телефона (формат +79995556677)
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone,
                @"^\+\d{11}$"))
                throw new ArgumentException("Телефон указан неверно");

            // Проверка паспорта (11 символов)
            if (passport.Length != 11)
                throw new ArgumentException("Паспорт должен содержать 11 символов");

            // Проверка ИНН
            ValidateInn(inn);

            // Проверка зарплаты
            if (salary <= 0)
                throw new ArgumentException("Зарплата должна быть положительной");

            // Хэширование пароля
            string passwordHash = HashPassword(password);

            // Вызов процедуры в базе данных
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
            call create_worker(
                {login}, {email}, {passwordHash}, {address}, {phone}, 
                {inn}, {passport}, {fullName}, {gender}, {position}, {salary}
            );");
        }


        public void Logout()
        {
            CurrentUser = null;
        }

        public int GetUserId()
        {
            return CurrentUser.UserId;
        }

        public string GetRole()
        {
            return CurrentUser.Role;
        }

        public string GetName()
        {
            return CurrentUser.Login;
        }
        public static void ValidateInn(string inn)
        {
            if (string.IsNullOrWhiteSpace(inn))
                throw new ArgumentException("ИНН не может быть пустым");

            inn = inn.Trim();
            inn = Regex.Replace(inn, @"\s+", "");

            if (!Regex.IsMatch(inn, @"^\d{10}$"))
                throw new ArgumentException($"ИНН некорректен: '{inn}', длина = {inn.Length}");
        }


        public bool IsWorker => CurrentUser?.Role == "worker";
        public bool IsReader => CurrentUser?.Role == "reader";

        public bool IsAdmin => CurrentUser?.Role == "admin";
    }
}

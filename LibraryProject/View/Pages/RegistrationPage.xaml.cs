using LibraryProject.Model;
using LibraryProject.Service;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages
{
    public partial class RegisterPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly AuthService _authService;

        public RegisterPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _authService = mainWindow.AuthService;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string phone = PhoneTextBox.Text;
            string address = AddressTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                System.Windows.MessageBox.Show("Логин, Email и пароль обязательны!");
                return;
            }

            if (password != confirmPassword)
            {
                System.Windows.MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                await _authService.Register(login, email, password, address, phone);
                System.Windows.MessageBox.Show("Регистрация успешна! Войдите в систему.");
                NavigationService?.Navigate(new LoginPage(_mainWindow));
            }
            catch (DbUpdateException)
            {
                System.Windows.MessageBox.Show("Пользователь с таким логином или email уже существует!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при регистрации: {ex.Message}  \nПопробуйте снова или перезапустите приложение.");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new LoginPage(_mainWindow));
        }
    }
}

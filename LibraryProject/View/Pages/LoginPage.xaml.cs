using LibraryProject.Service;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages
{
    public partial class LoginPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly AuthService _authService;

        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _authService = mainWindow.AuthService;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;

            if (_authService.Login(email, password))
            {
                _mainWindow.MainFrame.Navigate(new BooksPage(_mainWindow));
            }
            else
            {
                System.Windows.MessageBox.Show("Неверный email или пароль");
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new RegisterPage(_mainWindow));
        }
    }
}

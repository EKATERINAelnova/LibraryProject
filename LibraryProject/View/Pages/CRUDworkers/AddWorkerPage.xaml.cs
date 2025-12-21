using LibraryProject.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages.CRUDworkers
{
    public partial class AddWorkerPage : Page
    {
        private readonly MainWindow _main;
        private readonly AuthService _authService;

        public AddWorkerPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            _authService = main.AuthService;
        }

        private async void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string fullName = FullNameTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            string passport = PassportTextBox.Text.Trim();
            string inn = InnTextBox.Text.Trim();
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string position = PositionTextBox.Text.Trim();
            string salaryText = SalaryTextBox.Text.Trim();

            if (!decimal.TryParse(salaryText, out decimal salary))
            {
                System.Windows.MessageBox.Show("Некорректная зарплата!");
                return;
            }

            try
            {
                await _authService.RegisterWorker(
                    login, email, password, address, phone, inn, passport, fullName, gender, position, salary);

                System.Windows.MessageBox.Show("Сотрудник успешно добавлен!");
                _main.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}. \nПопробуйте снова или перезапустите приложение");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.GoBack();
        }
    }
}

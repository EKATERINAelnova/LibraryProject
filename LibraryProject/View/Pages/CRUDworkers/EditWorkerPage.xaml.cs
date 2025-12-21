using LibraryProject.Model;
using LibraryProject.Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages.CRUDworkers
{
    public partial class EditWorkerPage : Page
    {
        private readonly AuthService _authService;
        private readonly WorkerService _workerService;
        private readonly MainWindow _main;
        private readonly int _idworker;
        private readonly int _id;


        public EditWorkerPage(MainWindow main, Users worker, int id)
        {
            InitializeComponent();
            _main = main;
            _authService = _main.AuthService;
            _workerService = _main.WorkerService;
            _idworker = worker.UserId;
            _id = id;

            LoadWorkerData();
        }

        private async void LoadWorkerData()
        {
            var _worker = await _workerService.GetWorkerForEdit(_idworker);
            if (_worker == null) return;

            LoginTextBox.Text = _worker.Login;
            EmailTextBox.Text = _worker.Email;
            FullNameTextBox.Text = _worker.FullName;
            PhoneTextBox.Text = _worker.PhoneNumber;
            AddressTextBox.Text = _worker.Address;
            PassportTextBox.Text = _worker.Passport;
            InnTextBox.Text = _worker.Inn;
            PositionTextBox.Text = _worker.Position;
            SalaryTextBox.Text = _worker.Salary.ToString();

            if (_worker.Gender == "М")
                GenderComboBox.SelectedIndex = 0;
            else if (_worker.Gender == "Ж")
                GenderComboBox.SelectedIndex = 1;
        }

        private async void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string fullName = FullNameTextBox.Text.Trim();
                string phone = PhoneTextBox.Text.Trim();
                string address = AddressTextBox.Text.Trim();
                string passport = PassportTextBox.Text.Trim();
                string inn = InnTextBox.Text.Trim();
                string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string position = PositionTextBox.Text.Trim();
                string salaryText = SalaryTextBox.Text.Trim();

                // Валидация
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) ||
                    string.IsNullOrEmpty(address) || string.IsNullOrEmpty(passport) ||
                    string.IsNullOrEmpty(inn) || string.IsNullOrEmpty(gender) ||
                    string.IsNullOrEmpty(position) || string.IsNullOrEmpty(salaryText))
                {
                    System.Windows.MessageBox.Show("Все поля должны быть заполнены!");
                    return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    System.Windows.MessageBox.Show("Некорректный email!");
                    return;
                }

                if (!Regex.IsMatch(phone, @"^\+7\d{10}$"))
                {
                    System.Windows.MessageBox.Show("Телефон должен быть в формате +79995556677!");
                    return;
                }

                if (passport.Length != 11)
                {
                    System.Windows.MessageBox.Show("Паспорт должен содержать 11 символов!");
                    return;
                }

                if (!Regex.IsMatch(inn, @"^\d{10}$"))
                {
                    System.Windows.MessageBox.Show("ИНН должен содержать 10 цифр!");
                    return;
                }

                if (!decimal.TryParse(salaryText, out decimal salary))
                {
                    System.Windows.MessageBox.Show("Некорректная зарплата! Исправьте");
                    return;
                }

                // Обновление
                await _workerService.UpdateWorkerAsync(
                    _authService.GetUserId(),
                    _id,
                    login,
                    email,
                    address,
                    phone,
                    inn,
                    passport,
                    fullName,
                    gender,
                    position,
                    salary
                );

                System.Windows.MessageBox.Show("Сотрудник успешно обновлён!");
                _main.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при обновлении сотрудника: {ex.Message} \nПопробуйте снова или перезапустите приложение.");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на предыдущую страницу
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}

using LibraryProject.Model;
using LibraryProject.Service;
using LibraryProject.View.Pages.CRUDCopyBook;
using LibraryProject.View.Pages.CRUDworkers;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;


namespace LibraryProject.View.Pages
{
    public partial class UsersPage : Page
    {
        private readonly MainWindow _main;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private string selectedRole = null;

        private readonly ObservableCollection<Users> _users = new ObservableCollection<Users>();
        private int _currentPage = 1;
        private const int PageSize = 10;

        public UsersPage(MainWindow mainWindow)
        {
            _main= mainWindow;
            _authService = _main.AuthService;
            _userService = _main.UserService;

            InitializeComponent();
            RoleFilterComboBox.SelectionChanged += RoleFilterComboBox_SelectionChanged;
            _main.MainFrame.Navigated += MainFrame_Navigated; ;
            LoadUsers();
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is UsersPage)
            {
                LoadUsers();
            }
        }

        /// <summary>
        /// Загрузка пользователей с фильтром и пагинацией
        /// </summary>
        private async void LoadUsers()
        {
            if (_userService == null || RoleFilterComboBox == null)
                return;

            
            if (RoleFilterComboBox.SelectedItem is ComboBoxItem comboItem)
                selectedRole = comboItem.Content?.ToString();

            if (selectedRole == "Все")
                selectedRole = null;

            var usersPage = await _userService.GetUsersAsync(_currentPage, PageSize, selectedRole);

            _users.Clear();
            foreach (var user in usersPage)
                _users.Add(user);

            UsersDataGrid.ItemsSource = _users;
            PageInfoText.Text = $"Стр. {_currentPage}";

            // Обновление состояния кнопок после загрузки
            UpdateWorkerButtons(selectedRole);
        }

        /// <summary>
        /// Обработчик выбора роли в ComboBox
        /// </summary>
        private void RoleFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            LoadUsers();
        }

        /// <summary>
        /// Включение/выключение кнопок CRUD только для роли Worker
        /// </summary>
        private void UpdateWorkerButtons(string? selectedRole)
        {
            // Сбрасываем все кнопки
            AddWorkerButton.Visibility = Visibility.Collapsed;
            EditWorkerButton.Visibility = Visibility.Collapsed;
            DeleteWorkerButton.Visibility = Visibility.Collapsed;

            if (selectedRole == "Worker" && _authService.GetRole() == "admin")
            {
                AddWorkerButton.Visibility = Visibility.Visible;
                EditWorkerButton.Visibility = Visibility.Visible;
                DeleteWorkerButton.Visibility = Visibility.Visible;
            }
            if (selectedRole == "Worker" && _authService.GetRole() == "worker")
            {
                EditWorkerButton.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// Пагинация - предыдущая страница
        /// </summary>
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadUsers();
            }
        }

        /// <summary>
        /// Пагинация - следующая страница
        /// </summary>
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            LoadUsers();
        }

        /// <summary>
        /// Добавление работника
        /// </summary>
        private void AddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new AddWorkerPage(_main));
            LoadUsers();
        }

        /// <summary>
        /// Редактирование выбранного работника
        /// </summary>
        private void EditWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is Users selectedUser)
            {
                _main.MainFrame.Navigate(new EditWorkerPage(_main, selectedUser, selectedUser.UserId));
                LoadUsers();
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите работника для редактирования.");
            }
        }

        /// <summary>
        /// Удаление выбранного работника
        /// </summary>
        private void DeleteWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is Users selectedUser)
            {
                _main.MainFrame.Navigate(new DeleteWorkerPage(_main, selectedUser, _authService.GetUserId()));
                LoadUsers();
            }
            else
            {
                System.Windows.MessageBox.Show("Выберите работника для удаления.");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new BooksPage(_main));
        }

        private void Diagram_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new DiagramsPage(_main));
        }

    }
}

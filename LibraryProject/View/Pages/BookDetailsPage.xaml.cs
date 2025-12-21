using LibraryProject.Model;
using LibraryProject.Service;
using LibraryProject.View.Pages.CRUDCopyBook;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages
{
    public partial class BookDetailsPage : Page
    {
        private ObservableCollection<BooksCopyInfo> _copies = new();
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private readonly int _bookId;
        private readonly MainWindow _mainWindow;
        private int _currentPage = 1;
        private const int PageSize = 10;

        public BookDetailsPage(MainWindow mainWindow, int bookId)
        {
            InitializeComponent();
            _bookService = mainWindow.BookService;
            _authService = mainWindow.AuthService;
            _mainWindow = mainWindow;
            _bookId = bookId;
            if (_authService.IsReader)
            {
                EditBtn.Visibility = Visibility.Hidden;
                DeleteBtn.Visibility = Visibility.Hidden;
                AddBtn.Visibility = Visibility.Hidden;
            }
            if (_authService.IsWorker)
            {
               Reserve.Visibility = Visibility.Hidden;
            }

            LoadBookDetails();
        }

        private async void LoadBookDetails()
        {
            var copiesPage = await _bookService.GetCopiesAsync(_bookId, _currentPage, PageSize, _authService.IsWorker);

            _copies.Clear();

            foreach (var c in copiesPage)
                _copies.Add(c);
            
            CopiesDataGrid.ItemsSource = _copies;

            PageInfoText.Text = $"Стр. {_currentPage}";

            if (_copies.Any())
            {
                var firstCopy = _copies.First();

                titleBook. Text = firstCopy.BookTitle;
                authorsBook.Text = firstCopy.Authors;
                genresBook.Text = firstCopy.Genres;
            }
            else
            {
                // Если копий нет, показываем заглушку
                titleBook.Text = "Книга (копий нет)";
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                _copies.Clear();
                LoadBookDetails();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            _copies.Clear();
            LoadBookDetails();
        }

        private void LoadMore_Click(object sender, RoutedEventArgs e)
        {
            _copies.Clear();
            _currentPage = 1;
            LoadBookDetails();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BooksPage(_mainWindow));
        }
        private async void ReserveBook_Click(object sender, RoutedEventArgs e)
        {
            if (CopiesDataGrid.SelectedItem is not BooksCopyInfo selectedCopy)
            {
                System.Windows.MessageBox.Show("Выберите копию книги для бронирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int readerId = _mainWindow.AuthService.CurrentUser?.UserId ?? 0;
            if (readerId == 0)
            {
                System.Windows.MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Вызываем хранимую процедуру на уровне базы
                await _bookService.ReserveCopyAsync(readerId, selectedCopy.CopyId);

                System.Windows.MessageBox.Show("Запрос на бронирование отправлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем список доступных копий
                _copies.Clear();
                _currentPage = 1;
                LoadBookDetails();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Произошла ошибка: {ex.Message}  \nПопробуйте снова или перезапустите приложение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (CopiesDataGrid.SelectedItem is not BooksCopyInfo selectedCopy)
            {
                System.Windows.MessageBox.Show("Выберите копию книги для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Переходим на страницу редактирования
            _mainWindow.MainFrame.Navigate(new EditCopyPage(_mainWindow, selectedCopy, _bookId));
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (CopiesDataGrid.SelectedItem is not BooksCopyInfo selectedCopy)
            {
                System.Windows.MessageBox.Show("Выберите копию книги для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Переходим на страницу редактирования
            _mainWindow.MainFrame.Navigate(new DeleteCopyPage(_mainWindow, selectedCopy, _bookId));
        }

        private void AddCopy_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new AddCopyPage(_mainWindow, _bookId));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibraryProject.Model;
using LibraryProject.Service;
using LibraryProject.View.Pages.CRUDCopyBook;

namespace LibraryProject.View.Pages
{
    public partial class BooksPage : Page
    {
        private readonly BookService _bookService;
        private readonly MainWindow _mainWindow;
        private readonly AuthService _authService;

        private List<BookInfo> _books = new();
        private int _currentPage = 1;
        private const int PageSize = 5;

        private int _totalBooks;
        private int _maxPages;

        public BooksPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _authService = mainWindow.AuthService;
            _bookService = mainWindow.BookService;
            _mainWindow = mainWindow;
            if (!_authService.IsReader)
            {
                MyBooks.Visibility = Visibility.Hidden;
            }
            if (!_authService.IsWorker && !_authService.IsAdmin)
            {
                MyLoans.Visibility = Visibility.Hidden;
                AllRequests.Visibility = Visibility.Hidden;
                UserBtn.Visibility = Visibility.Hidden;
            }
            Loaded += BooksPage_Loaded;
        }

        private async void BooksPage_Loaded(object sender, RoutedEventArgs e)
        {
            _totalBooks = await _bookService.GetTotalBooksCount();
            _maxPages = (int)Math.Ceiling(_totalBooks / (double)PageSize);
            role.Text = "Роль: " + _authService.GetRole();
            name.Text = "Имя: " + _authService.GetName();
            await LoadBooks();
        }

        private async Task LoadBooks()
        {
            _books = await _bookService.GetBooks(_currentPage, PageSize);

            BooksDataGrid.ItemsSource = null;
            BooksDataGrid.ItemsSource = _books;

            PageInfoText.Text = $"Стр. {_currentPage} из {_maxPages}";
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadBooks();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _maxPages)
            {
                _currentPage++;
                await LoadBooks();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();

            var filtered = _books.Where(b =>
                b.Title.ToLower().Contains(search) ||
                (b.Authors != null && b.Authors.ToLower().Contains(search))
            ).ToList();

            BooksDataGrid.ItemsSource = filtered;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            BooksDataGrid.ItemsSource = _books;
        }

        private void BooksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is BookInfo selectedBook)
            {
                NavigationService?.Navigate(new BookDetailsPage(_mainWindow, selectedBook.BookId));
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _authService.Logout();
            _mainWindow.MainFrame.Navigate(new LoginPage(_mainWindow));
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new MyReservedPage(_mainWindow));
        }

        private void AllRequsts_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new AllRequestsPage(_mainWindow));
        }

        private void MyLoans_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new LoansPage(_mainWindow));
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new UsersPage(_mainWindow));
        }
    }
}

using LibraryProject.Model;
using LibraryProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages
{
    public partial class AllRequestsPage : Page
    {
        private readonly LoanService _loanService;
        private readonly AuthService _authService;
        private readonly MainWindow _mainWindow;
        private List<RequestsInfo> _allReqs = new();
        private List<RequestsInfo> _filteredReqs = new();
        private int _currentPage = 1;
        private const int PageSize = 10;
        private int _totalReqs;
        private int _maxPages;

        public AllRequestsPage(MainWindow main)
        {
            _mainWindow = main;
            _loanService = _mainWindow.LoanService;
            _authService = _mainWindow.AuthService;
            InitializeComponent();
            Loaded += ReqsPage_Loaded;
        }

        private async void ReqsPage_Loaded(object sender, RoutedEventArgs e)
        {
            _allReqs = await _loanService.GetReqs(0, 0);
            _filteredReqs = new List<RequestsInfo>(_allReqs);

            _totalReqs = _filteredReqs.Count;
            _maxPages = (int)Math.Ceiling(_totalReqs / (double)PageSize);

            await LoadPage();
        }

        private async Task LoadPage()
        {
            var pageItems = _filteredReqs
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            BooksDataGrid.ItemsSource = pageItems;
            PageInfoText.Text = $"Стр. {_currentPage} из {_maxPages}";
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                _filteredReqs = new List<RequestsInfo>(_allReqs);
            }
            else
            {
                _filteredReqs = _allReqs.Where(r =>
                    (r.BookTitle != null && r.BookTitle.ToLower().Contains(search)) ||
                    (r.UserName != null && r.UserName.ToLower().Contains(search)) ||
                    (r.Authors != null && r.Authors.ToLower().Contains(search))
                ).ToList();
            }

            _currentPage = 1;
            _totalReqs = _filteredReqs.Count;
            _maxPages = (int)Math.Ceiling(_totalReqs / (double)PageSize);
            await LoadPage();
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            _filteredReqs = new List<RequestsInfo>(_allReqs);
            _currentPage = 1;
            _totalReqs = _filteredReqs.Count;
            _maxPages = (int)Math.Ceiling(_totalReqs / (double)PageSize);
            await LoadPage();
        }

        private async void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadPage();
            }
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _maxPages)
            {
                _currentPage++;
                await LoadPage();
            }
        }

        private async void ReqToLoan_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is not RequestsInfo selectedReq)
            {
                System.Windows.MessageBox.Show("Выберите запрос для рассмотрения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"Одобрить запрос на книгу '{selectedReq.BookTitle}' для читателя {selectedReq.UserName}?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            try
            {
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        await _loanService.ApproveRequestAsync(selectedReq.RequestId, _authService.CurrentUser.UserId);

                        System.Windows.MessageBox.Show("Запрос одобрен, книга выдана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case MessageBoxResult.No:
                        await _loanService.RejectRequestAsync(selectedReq.RequestId);

                        System.Windows.MessageBox.Show("Запрос отклонен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }

                // После изменения статуса обновляем данные
                _allReqs = await _loanService.GetReqs(0, 0);
                _filteredReqs = new List<RequestsInfo>(_allReqs);
                _currentPage = 1;
                _totalReqs = _filteredReqs.Count;
                _maxPages = (int)Math.Ceiling(_totalReqs / (double)PageSize);
                await LoadPage();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Произошла ошибка: {ex.Message} \nПопробуйте снова или перезапустите приложение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new BooksPage(_mainWindow));
        }
    }
}

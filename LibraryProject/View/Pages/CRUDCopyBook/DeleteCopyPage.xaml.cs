using System.Windows;
using System.Windows.Controls;
using LibraryProject.Model;
using LibraryProject.Service;

namespace LibraryProject.View.Pages.CRUDCopyBook
{
    public partial class DeleteCopyPage : Page
    {
        private readonly BookService _bookService;
        private readonly MainWindow _main;
        private readonly AuthService _authService;
        private readonly BooksCopyInfo _copy;
        private readonly int _idBook;

        public DeleteCopyPage(MainWindow main, BooksCopyInfo copy, int bookId)
        {
            InitializeComponent();

            _main = main;
            _bookService = main.BookService;
            _authService = main.AuthService;
            _copy = copy;
            _idBook = bookId;

            InfoText.Text = $"Удалить экземпляр №{copy.CopyId} книги \"{copy.BookTitle}\"?";
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _bookService.DeleteCopyAsync(_copy.CopyId, _authService.GetUserId());

                System.Windows.MessageBox.Show("Экземпляр удалён", "Успех");

                _main.MainFrame.Navigate(new BookDetailsPage(_main, _idBook));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка: " + ex.Message + "\nПопробуйте снова или перезапустите приложение.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new BookDetailsPage(_main, _idBook));
        }
    }
}

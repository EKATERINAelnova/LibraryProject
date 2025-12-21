using LibraryProject.Data;
using LibraryProject.Model;
using LibraryProject.Service;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibraryProject.View.Pages.CRUDCopyBook
{
    public partial class EditCopyPage : Page
    {
        private readonly BookService _bookService;
        private readonly MainWindow _main;
        private readonly AuthService _authService;
        private readonly BooksCopyInfo _copy;
        private readonly int _idBook;

        public EditCopyPage(MainWindow main, BooksCopyInfo copy, int idBook)
        {
            InitializeComponent();

            _main = main;
            _bookService = main.BookService;
            _authService = main.AuthService;
            _copy = copy;
            _idBook = idBook;

            Loaded += EditCopyPage_Loaded;
        }

        private async void EditCopyPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            // Текстовые поля
            YearBox.Text = _copy.YearPublish?.ToString() ?? "";
            ClosetBox.Text = _copy.Closet.ToString();
            ShelfBox.Text = _copy.Shelf.ToString();
            PlaceBox.Text = _copy.Place.ToString();

            // ComboBoxs
            var publishers = await _bookService.GetAllPublishersAsync();
            PublisherBox.ItemsSource = publishers;
            PublisherBox.DisplayMemberPath = "Name";
            PublisherBox.SelectedItem = publishers.FirstOrDefault(p => p.Name.ToUpper() == _copy.Publisher.ToUpper());

            var sections = await _bookService.GetAllSectionsAsync();
            SectionBox.ItemsSource = sections;
            SectionBox.DisplayMemberPath = "Name";
            SectionBox.SelectedItem = sections.FirstOrDefault(s => s.Name.ToUpper() == _copy.Section.ToUpper());

            var statuses = await _bookService.GetAllCopyStatusesAsync();
            StatusBox.ItemsSource = statuses;
            StatusBox.DisplayMemberPath = "Name";
            StatusBox.SelectedItem = statuses.FirstOrDefault(s => s.Name.ToUpper() == _copy.Status.ToUpper());
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка ComboBox
                if (PublisherBox.SelectedItem == null ||
                    SectionBox.SelectedItem == null ||
                    StatusBox.SelectedItem == null)
                {
                    System.Windows.MessageBox.Show("Все поля должны быть заполнены!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Год издания
                if (!int.TryParse(YearBox.Text, out int year) ||
                    year < 1500 || year > DateTime.Now.Year)
                {
                    System.Windows.MessageBox.Show("Введите корректный год издания!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Шкаф
                if (!int.TryParse(ClosetBox.Text, out int closet) || closet <= 0)
                {
                    System.Windows.MessageBox.Show("Номер шкафа должен быть положительным числом!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Полка
                if (!int.TryParse(ShelfBox.Text, out int shelf) || shelf <= 0)
                {
                    System.Windows.MessageBox.Show("Номер полки должен быть положительным числом!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Место
                if (!int.TryParse(PlaceBox.Text, out int place) || place <= 0)
                {
                    System.Windows.MessageBox.Show("Номер места должен быть положительным числом!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string publisherName = (PublisherBox.SelectedItem as dynamic).Name;
                string sectionName = (SectionBox.SelectedItem as dynamic).Name;
                string statusName = (StatusBox.SelectedItem as dynamic).Name;

                await _bookService.UpdateCopyAsync(
                    _copy.CopyId,
                    publisherName,
                    sectionName,
                    year,
                    closet,
                    shelf,
                    place,
                    statusName,
                    _authService.GetUserId()
                );

                System.Windows.MessageBox.Show("Экземпляр обновлён",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                _main.MainFrame.Navigate(new BookDetailsPage(_main, _idBook));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    "Ошибка при обновлении экземпляра:\n" + ex.Message +
                    "\nПопробуйте снова или перезапустите приложение.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new BookDetailsPage(_main, _idBook));
        }
    }
}

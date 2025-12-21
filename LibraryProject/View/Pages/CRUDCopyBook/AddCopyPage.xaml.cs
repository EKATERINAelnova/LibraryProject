using LibraryProject.Data;
using LibraryProject.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Formats.Asn1.AsnWriter;


namespace LibraryProject.View.Pages.CRUDCopyBook
{
    /// <summary>
    /// Логика взаимодействия для AddCopyPage.xaml
    /// </summary>
    public partial class AddCopyPage : Page
    {
        private readonly MainWindow _main;
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private readonly int _bookId;

        public AddCopyPage(MainWindow main, int idBook)
        {
            InitializeComponent();
            _main = main;
            _bookId = idBook;
            _bookService = main.BookService;
            _authService = main.AuthService;

            Loaded += AddCopyPage_Loaded;
        }

        private async void AddCopyPage_Loaded(object sender, RoutedEventArgs e)
        {

            PublisherCombo.ItemsSource = await _bookService.GetAllPublishersAsync();
            PublisherCombo.DisplayMemberPath = "Name";
            PublisherCombo.SelectedValuePath = "PublisherId";

            SectionCombo.ItemsSource = await _bookService.GetAllSectionsAsync();
            SectionCombo.DisplayMemberPath = "Name";
            SectionCombo.SelectedValuePath = "SectionId";

            StatusCombo.ItemsSource = await _bookService.GetAllCopyStatusesAsync();
            StatusCombo.DisplayMemberPath = "Name";
            StatusCombo.SelectedValuePath = "StatusId";
        }

        private async void AddCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверки
                if (PublisherCombo.SelectedValue == null ||
                    SectionCombo.SelectedValue == null ||
                    StatusCombo.SelectedValue == null)
                {
                    System.Windows.MessageBox.Show("Все поля должны быть заполнены!");
                    return;
                }

                if (!int.TryParse(YearText.Text, out int year) ||
                    year < 1500 || year > DateTime.Now.Year)
                {
                    System.Windows.MessageBox.Show("Введите корректный год издания!");
                    return;
                }

                if (!int.TryParse(ClosetText.Text, out int closet) || closet <= 0)
                {
                    System.Windows.MessageBox.Show("Номер шкафа должен быть положительным числом!");
                    return;
                }

                if (!int.TryParse(ShelfText.Text, out int shelf) || shelf <= 0)
                {
                    System.Windows.MessageBox.Show("Номер полки должен быть положительным числом!");
                    return;
                }

                if (!int.TryParse(PlaceText.Text, out int place) || place <= 0)
                {
                    System.Windows.MessageBox.Show("Номер места должен быть положительным числом!");
                    return;
                }

                if (!int.TryParse(NumOfPages.Text, out int numOfPages) ||
                    numOfPages <= 0 || numOfPages > 5000)
                {
                    System.Windows.MessageBox.Show("Введите корректное количество страниц!");
                    return;
                }

                await _bookService.AddCopyAsync(
                    _bookId,
                    PublisherCombo.Text,
                    SectionCombo.Text,
                    year,
                    closet,
                    shelf,
                    place,
                    StatusCombo.Text,
                    numOfPages,
                    _authService.GetUserId()
                );

                System.Windows.MessageBox.Show("Экземпляр добавлен успешно!");
                _main.MainFrame.Navigate(new BooksPage(_main));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    "Ошибка при добавлении экземпляра:\n" + ex.Message +
                    "\nПопробуйте снова или перезапустите приложение."
                );
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new BookDetailsPage(_main, _bookId));
        }
    }
}

using LibraryProject.Model;
using LibraryProject.Service;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryProject.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для MyReservedPage.xaml
    /// </summary>
    public partial class MyReservedPage : Page
    {
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private readonly MainWindow _main;
        public MyReservedPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            _bookService = main.BookService;
            _authService = main.AuthService;

            Loaded += UserRequestsPage_Loaded;
        }

        private async void UserRequestsPage_Loaded(object sender, RoutedEventArgs e)
        {
            int id = _authService.GetUserId();

            var requests = await _bookService.GetUserRequests(id);

            RequestsGrid.ItemsSource = requests;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new BooksPage(_main));
        }

        private async void DeleteReserve_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsGrid.SelectedItem is UserRequestsInfo selectedRequest)
            {
                if (selectedRequest.Status.ToLower() != "на рассмотрении")
                {
                    System.Windows.MessageBox.Show("Эту бронь нельзя отменить, так как книга уже в другом статусе.");
                    return;
                }

                await _bookService.CancelReservationAsync(selectedRequest.Request_Id);

                int userId = _authService.GetUserId();
                RequestsGrid.ItemsSource = await _bookService.GetUserRequests(userId);
            }
        }




    }
}

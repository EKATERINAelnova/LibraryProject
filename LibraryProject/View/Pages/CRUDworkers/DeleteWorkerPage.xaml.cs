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
using static System.Formats.Asn1.AsnWriter;

namespace LibraryProject.View.Pages.CRUDworkers
{
    /// <summary>
    /// Логика взаимодействия для DeleteWorkerPage.xaml
    /// </summary>
    public partial class DeleteWorkerPage : Page
    {
        private readonly WorkerService _workerService;
        private readonly MainWindow _main;
        private readonly AuthService _authService;
        private readonly Users _user;
        private readonly int _idUser;

        public DeleteWorkerPage(MainWindow main, Users user, int userId)
        {
            InitializeComponent();

            _main = main;
            _workerService = main.WorkerService;
            _authService = main.AuthService;
            _user = user;
            _idUser = userId;

            InfoText.Text = $"Удалить экземпляр №{user.UserId} книги \"{user.Login}\"?";
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _workerService.DeleteUserAsync(_user.UserId, _authService.GetUserId());

                System.Windows.MessageBox.Show("Экземпляр удалён", "Успех");

                _main.MainFrame.Navigate(new UsersPage(_main));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка: " + ex.Message + "\nПопробуйте снова или перезапустите приложение.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _main.MainFrame.Navigate(new UsersPage(_main));
        }
    }
}

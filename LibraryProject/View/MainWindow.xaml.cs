using LibraryProject.Data;
using LibraryProject.Service;
using LibraryProject.View.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace LibraryProject.View
{
    public partial class MainWindow : Window
    {
        public LibraryContext DbContext { get; }
        public AuthService AuthService { get; }
        public BookService BookService { get; }
        public LoanService LoanService { get; }
        public UserService UserService { get; }
        public WorkerService WorkerService { get; }

        public MainWindow()
        {
            InitializeComponent();
            DbContext = new LibraryContext();
            AuthService = new AuthService(DbContext);
            BookService = new BookService(DbContext);
            LoanService = new LoanService(DbContext);
            UserService = new UserService();
            WorkerService = new WorkerService(DbContext);
            MainFrame.Navigate(new LoginPage(this));
        }
    }
}
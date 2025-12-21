using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Security.Policy;

namespace LibraryProject.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
        {
        }

        public DbSet<Books> Books => Set<Books>();
        public DbSet<CopyBooks> CopyBooks => Set<CopyBooks>();
        public DbSet<Authors> Authors => Set<Authors>();
        public DbSet<Genres> Genres => Set<Genres>();
        public DbSet<Sections> Sections => Set<Sections>();
        public DbSet<Model.Publishers> Publishers => Set<Model.Publishers>();
        public DbSet<Countries> Countries => Set<Countries>();
        public DbSet<Users> Users => Set<Users>();

        public DbSet<Readers> Readers => Set<Readers>();
        public DbSet<Workers> Workers => Set<Workers>();
        public DbSet<BookLoans> BookLoans => Set<BookLoans>();
        public DbSet<BookGenres> ContainIns => Set<BookGenres>();
        public DbSet<BookAuthors> Writes => Set<BookAuthors>();
        public DbSet<Requests> Requests => Set<Requests>();
        public DbSet<CopyStatus> CopyStatus => Set<CopyStatus>();
        public DbSet<RequestsInfo> RequestsInfo => Set<RequestsInfo>();
        public DbSet<BooksCopyInfo> BooksCopyInfo => Set<BooksCopyInfo>();
        public DbSet<BookInfo> BooksInfo => Set<BookInfo>();
        public DbSet<UserRequestsInfo> UserRequestsInfo { get; set; }
        public DbSet<StatReaders> StatReaders => Set<StatReaders>();
        public DbSet<StatWorkers> StatWorkers => Set<StatWorkers>();
        public DbSet<WorkersInfo> WorkersInfo => Set<WorkersInfo>();

        

        public DbSet<BookLoanInfo> BookLoanInfo => Set<BookLoanInfo>();
        public LibraryContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=5942e-rw.db.pub.dbaas.postgrespro.ru:5432;Database=dbproject;Username=elnova_ed;Password=#2c0qsv8yP8");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Добавляем настройку для результата функции
            modelBuilder.Entity<UserRequestsInfo>().HasNoKey();
            modelBuilder.Entity<StatReaders>().HasNoKey();
            modelBuilder.Entity<StatWorkers>().HasNoKey();
        }

    }
}

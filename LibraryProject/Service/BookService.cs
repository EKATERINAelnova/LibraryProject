using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace LibraryProject.Service
{
    public class BookService
    {
        private readonly LibraryContext _db;
        public BookService(LibraryContext db) => _db = db;

        public async Task<List<BookInfo>> GetBooks(int page, int pageSize)
        {
            return await _db.BooksInfo
                .OrderBy(b => b.BookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Получить доступные копии книги по id через функцию базы данных
        /// </summary>
        /// <param name="bookId">Id книги</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Список доступных копий книги</returns>
        public async Task<List<BooksCopyInfo>> GetCopiesAsync(int bookId, int page, int pageSize, bool isWorker)
        {
            _db.ChangeTracker.Clear();

            var allCopies = await _db.BooksCopyInfo
                .FromSqlInterpolated(
                    $"select * from get_book_copies_info({bookId}, {isWorker})")
                .ToListAsync();

            return allCopies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }


        public async Task<int> GetTotalBooksCount()
        {
            return await _db.BooksInfo.CountAsync();
        }

        public async Task ReserveCopyAsync(int readerId, int copyId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call create_reservation({readerId}, {copyId})");
        }

        public async Task<List<UserRequestsInfo>> GetUserRequests(int userId)
        {
            return await _db.UserRequestsInfo
                .FromSqlInterpolated($"select * from get_user_requests({userId})")
                .ToListAsync();
        }
        public async Task CancelReservationAsync(int requestId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call cancel_reservation({requestId})");
        }

        public async Task AddCopyAsync(
            int bookId,
            string publisherName,
            string sectionName,
            int year,
            int closet,
            int shelf,
            int place,
            string statusName, int num_of_pages, int userId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"call add_book_copy({bookId}, {publisherName}, {sectionName}, {year}, {closet}, {shelf}, {place}, {statusName}, {num_of_pages}, {userId})"
                );
        }

        public async Task UpdateCopyAsync(
            int copyId,
            string publisherName,
            string sectionName,
            int year,
            int closet,
            int shelf,
            int place,
            string statusName, int userId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"CALL update_copy({copyId}, {publisherName}, {sectionName}, {year}, {closet}, {shelf}, {place}, {statusName}, {userId})");
        }



        public async Task DeleteCopyAsync(int copyId, int userId)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "CALL delete_copy({0}, {1})", copyId, userId);

        }

        public async Task<List<Books>> GetAllBooksAsync()
        {
            return await _db.Books.OrderBy(b => b.Title).ToListAsync();
        }

        public async Task<List<Authors>> GetAllAuthorsAsync()
        {
            return await _db.Authors.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<List<Publishers>> GetAllPublishersAsync()
        {
            return await _db.Publishers.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<Sections>> GetAllSectionsAsync()
        {
            return await _db.Sections.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<CopyStatus>> GetAllCopyStatusesAsync()
        {
            return await _db.CopyStatus.OrderBy(s => s.Name).ToListAsync();
        }


    }
}

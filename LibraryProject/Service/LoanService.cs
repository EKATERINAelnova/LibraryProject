using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Service
{
    public class LoanService
    {
        private readonly LibraryContext _db;

        public LoanService(LibraryContext db) => _db = db;

        public async Task<List<RequestsInfo>> GetReqs(int page, int pageSize)
        {
            return await _db.RequestsInfo
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }
        public async Task<int> GetTotalReqsCount()
        {
            return await _db.RequestsInfo.CountAsync();
        }
        public async Task ApproveRequestAsync(int requestId, int workerId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call approve_request({workerId},{requestId})");
        }

        public async Task RejectRequestAsync(int requestId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call reject_request({requestId})");
        }

        public async Task<List<BookLoanInfo>> GetLoansAsync()
        {
            _db.ChangeTracker.Clear();
            return await _db.BookLoanInfo
                .OrderByDescending(r => r.LoanDate)
                .ToListAsync();
        }

        public async Task ReturnLoanAsync(int loanId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"call return_book({loanId})"
            );
        }

    }
}

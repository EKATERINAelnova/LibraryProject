using Azure.Core;
using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LibraryProject.Service
{
    public class UserService
    {
        public async Task<IEnumerable<StatWorkers>> GetWorkersStatsAsync()
        {
            await using var db = new LibraryContext();

            return await db.StatWorkers
                .FromSqlRaw("select * from stat_workers")
                .AsNoTracking()
                .OrderByDescending(s => s.LoansCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<StatReaders>> GetReadersStatsAsync()
        {
            await using var db = new LibraryContext();

            return await db.StatReaders
                .FromSqlRaw("select * from stat_readers")
                .AsNoTracking()
                .OrderByDescending(s => s.Count)
                .ToListAsync();
        }

        public async Task<List<Users>> GetUsersAsync(int page, int pageSize, string? role = null)
        {
            await using var db = new LibraryContext();
            db.ChangeTracker.Clear();

            IQueryable<Users> query = db.Users.AsNoTracking();
            query = query.Where(s => s.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role.ToLower() == role.ToLower());

            return await query
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }

}

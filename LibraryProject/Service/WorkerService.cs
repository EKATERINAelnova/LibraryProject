using LibraryProject.Data;
using LibraryProject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryProject.Service
{
    public class WorkerService
    {
        private readonly LibraryContext _db;
        public WorkerService(LibraryContext db) => _db = db;

        public async Task DeleteUserAsync(int delUserId, int userId)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "CALL delete_worker({0}, {1})", delUserId, userId);

        }
        public async Task UpdateWorkerAsync( int userId, int id,
            string login,
            string email,
            string address,
            string phone,
            string inn,
            string passport,
            string fullName,
            string gender,
            string position,
            decimal salary)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                CALL update_worker( {userId},{id},
                    {login},
                    {email},
                    {address},
                    {phone},
                    {inn},
                    {passport},
                    {fullName},
                    {gender},
                    {position},
                    {salary})");
        }


        public async Task<WorkersInfo> GetWorkerForEdit(int id)
        {
            return await _db.Users
                .Where(u => u.UserId == id)
                .Select(u => new WorkersInfo
                {
                    UserId = u.UserId,
                    Login = u.Login,
                    Email = u.Email,
                    FullName = u.Worker.FullName,
                    Phone = u.Worker.PhoneNumber,
                    Inn = u.Worker.Inn,
                    Passport = u.Worker.Passport,
                    Address = u.Worker.Address,
                    PhoneNumber = u.Worker.PhoneNumber,
                    Gender = u.Worker.Gender,
                    Position = u.Worker.Position,
                    Salary = u.Worker.Salary
                })
                .FirstAsync();
        }

    }
}

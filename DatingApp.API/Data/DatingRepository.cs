using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context) => _context = context;
        public void Add<T>(T entity) where T : class => _context.Add(entity);

        public void Delete<T>(T entity) where T : class => _context.Remove(entity);

        public async Task<Photo> GetMainPhotoForUser(int userId) => await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);

        public async Task<Photo> GetPhoto(int id) => await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<User> GetUser(int id) =>
        await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(u => u.Id == id);
        public async Task<bool> SaveAll() => await _context.SaveChangesAsync() > 0;
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //here we don't execute the retrival from the context, it's a sync method and we remove the wait            
            var users = _context.Users.Include(x => x.Photos).OrderByDescending(u => u.LastActive).AsQueryable()
            .Where(u => u.Id != userParams.UserId)
            .Where(u => u.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth > minDob && u.DateOfBirth < maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {   
                switch (userParams.OrderBy.ToLower())
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;

                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }

            }


            //it's here when we create async PageList and using IQueryable we add all pagination 
            //We get current page and returned back to the UserControler/GetUser 
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
    }
}
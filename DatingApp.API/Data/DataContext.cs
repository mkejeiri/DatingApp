using DatingApp.API.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext //represent a session with a Database
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            // if (options == null)
            // {
            //     throw new System.ArgumentNullException(nameof(options));
            // }
        }
        //to tell entity framework about our entities we need to give properties
        public DbSet<Value> Values { get; set; }

    }
}
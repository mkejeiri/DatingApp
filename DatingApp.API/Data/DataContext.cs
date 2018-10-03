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
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder){

            //form the PK: LikerId + LikeeId
            modelBuilder.Entity<Like>()
            .HasKey(k => new {k.LikerId, k.LikeeId});

            //Liker could have many likees : 
            //e.g : a user (likee) could like many users and could be liked by many other users (likers)
            modelBuilder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey( f => f.LikerId)
            .OnDelete(DeleteBehavior.Restrict);

            //Likee could have many likers
            //e.g : a user (likee) could like many users and could be liked by many other users (likers)
            modelBuilder.Entity<Like>()
            .HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(f => f.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
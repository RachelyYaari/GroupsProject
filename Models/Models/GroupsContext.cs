using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Models.Models
{
    public class GroupsContext : DbContext
    {
        public GroupsContext(DbContextOptions<GroupsContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Event> Events { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasMany(p => p.Events)
                .WithMany(e => e.ConfirmedArrival)
                .UsingEntity(j => j.ToTable("UserEvent")); // הגדרת טבלת הקשר בין User ל־Event

            modelBuilder.Entity<User>()
                .HasMany(p => p.Groups)
                .WithMany(g => g.Members)
                .UsingEntity(j => j.ToTable("UserGroup")); // הגדרת טבלת הקשר בין User ל־Group

            base.OnModelCreating(modelBuilder);
        }

    }
}
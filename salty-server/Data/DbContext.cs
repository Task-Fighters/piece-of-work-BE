using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using salty_server.Models;

    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext (DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<Assignment> Assignments { get; set; } = default!;
        public DbSet<GroupUser> GroupUser { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
            .HasMany(p => p.Users)
            .WithMany(p => p.Groups)
            .UsingEntity<GroupUser>(
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.GroupUsers)
                    .HasForeignKey(pt => pt.UsersId),
                j => j
                    .HasOne(pt => pt.Group)
                    .WithMany(p => p.GroupUsers)
                    .HasForeignKey(pt => pt.GroupsId),
                j =>
                {
                    j.HasKey(t => new { t.UsersId, t.GroupsId });
                });
        }
    }


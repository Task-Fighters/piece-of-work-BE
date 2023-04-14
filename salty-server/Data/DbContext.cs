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

        // protected override void OnModelCreating(DbModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Recipe>()
        //     .HasMany(p => p.Users)
        //     .WithMany(p => p.Recipes)
        //     `.UsingEntity<RecipeUser>(
        //         j => j
        //             .HasOne(pt => pt.User)
        //             .WithMany(t => t.RecipeUsers)
        //             .HasForeignKey(pt => pt.UserId),
        //         j => j
        //             .HasOne(pt => pt.Recipe)
        //             .WithMany(p => p.RecipeUsers)
        //             .HasForeignKey(pt => pt.RecipeId),
        //         j =>
        //         {
        //             j.HasKey(t => new { t.UserId, t.RecipeId });
        //         });
        // }

    }


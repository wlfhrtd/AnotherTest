using DAL.Exceptions;
using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EfStructures
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            SavingChanges += (sender, args)
                => Console.WriteLine($"Saving changes for {((ApplicationDbContext)sender)!.Database!.GetConnectionString()}");

            SavedChanges += (sender, args)
                => Console.WriteLine(
                    $"Saved {args!.EntitiesSavedCount} changes" +
                    $" for {((ApplicationDbContext)sender)!.Database!.GetConnectionString()}");

            SaveChangesFailed += (sender, args)
                => Console.WriteLine($"An exception occurred! {args.Exception.Message} entities");

            ChangeTracker.Tracked += ChangeTracker_Tracked;
            ChangeTracker.StateChanged += ChangeTracker_StateChanged;
        }


        public DbSet<Department>? Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Departments)
                .WithMany(e => e.Subdepartments)
                .UsingEntity<DepartmentMap>(
                e => e.HasOne<Department>().WithMany().HasForeignKey(e => e.DepartmentName),
                e => e.HasOne<Department>().WithMany().HasForeignKey(e => e.SubdepartmentName));

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new CustomConcurrencyException("Concurrency exception occurred", e);
            }
            catch (RetryLimitExceededException e)
            {
                // DbResiliency retry limit exceeded
                throw new CustomRetryLimitExceededException("RetryLimitExceeded exception occurred; check db server", e);
            }
            catch (Exception e)
            {
                throw new CustomException("An error occurred while updating the database", e);
            }
        }

        private void ChangeTracker_Tracked(object? sender, EntityTrackedEventArgs args)
        {
            var source = (args.FromQuery) ? "Database" : "Code";
            if (args.Entry.Entity is Department d)
            {
                Console.WriteLine($"Department entry {d.Name} was added from {source}");
            }
        }

        private void ChangeTracker_StateChanged(object? sender, EntityStateChangedEventArgs args)
        {
            if (args.Entry.Entity is not Department d)
            {
                return;
            }

            Console.WriteLine($"Department {d.Name} had been {args.OldState} until state changed to {args.NewState}");

            var action = string.Empty;
            switch (args.NewState)
            {
                case EntityState.Unchanged:
                    action = args.OldState switch
                    {
                        EntityState.Added => "Added",
                        EntityState.Modified => "Edited",
                        _ => action,
                    };
                    Console.WriteLine($"The object was {action}");
                    break;
            }
        }
    }
}

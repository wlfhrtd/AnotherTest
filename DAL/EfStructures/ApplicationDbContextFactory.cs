using DAL.EfStructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;


namespace Dal.EfStructures
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
            string connectionString = @"Server=127.0.0.1;Port=5432;Database=SafibTest;User Id=postgres;Password=P@ssw0rd;Include Error Detail=true";
            optionsBuilder.UseNpgsql(connectionString);
            Console.WriteLine($"Connecting to: {connectionString}");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

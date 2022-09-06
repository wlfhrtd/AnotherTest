using DAL.EfStructures;
using DAL.Repositories.Base;
using DAL.Repositories.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAL.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context) { }

        internal DepartmentRepository(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public override IEnumerable<Department> FindAll()
            => Table
            .Include(d => d.Subdepartments);

        public IEnumerable<Department> FindAllBy(string subdepartmentName)
            => Table
            .Where(d => d.Subdepartments.Any(sub => d.Name == subdepartmentName))
            .Include(d => d.Subdepartments);

        public Department? FindOneByName(string? name)
            => Table
            .Where(d => d.Name == name)
            .Include(d => d.Subdepartments)
            .FirstOrDefault();

        public async Task<List<Department>> FindAllAsNoTrackingAsync(string? name)
        {
            var departments = from d in Context.Departments
                              select d;

            if (!string.IsNullOrEmpty(name))
            {
                // IQueryable database implementation ~ case-sensitive by default
                departments = departments
                    .Where(d => d.Name.Contains(name));
            }

            return await departments.AsNoTracking().ToListAsync();
        }

        public async Task<List<Department>> FindAllAsNoTrackingIncludeAsync()
            => await Table.Include(d => d.Subdepartments).ToListAsync();

        public override async Task<Department?> FindOneByNameAsync(string name)
        {
            return await Table
                 .Where(d => d.Name == name)
                 .Include("Subdepartments")
                 .SingleOrDefaultAsync();
        }

        public async Task<Department?> FindFirstOrDefaultByNameAsync(string name)
            => await Table.FirstOrDefaultAsync(d => d.Name == name);


        public async Task<List<string>> FindAllDepartmentNamesAsync()
        {
            return await (from d in Table
                          select d.Name).ToListAsync();
        }

        public async Task<Department> FindSingleByNameNoIncludeAsync(string name)
        {
            return await Table.SingleAsync(d => d.Name == name);
        }

        public async Task<Department> FindSingleByNameWithIncludeAsync(string name)
        {
            return await Table
                .Include(d => d.Subdepartments)
                .SingleAsync(d => d.Name == name);
        }

        public IQueryable<Department>? FindAllWithMatchingNames(IList<string> departmentsNames)
        {
            return Table.Where(d => departmentsNames.Any(inc => inc == d.Name));
        }

        public bool DepartmentExists(string name)
        {
            return (Context.Departments?.Any(e => e.Name == name)).GetValueOrDefault();
        }
    }
}

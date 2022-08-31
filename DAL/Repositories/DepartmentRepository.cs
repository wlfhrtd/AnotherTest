using DAL.EfStructures;
using DAL.Repositories.Base;
using DAL.Repositories.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context) { }

        internal DepartmentRepository(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public override IEnumerable<Department> FindAll()
            => Table
            .Include(d => d.Subdepartments);

        public IEnumerable<Department> FindAllBy(int departmentId)
            => Table
            .Where(d => d.Id == departmentId)
            .Include(d => d.Subdepartments);

        public override Department? FindOneById(int? id)
            => Table
            .Where(d => d.Id == id)
            .Include(d => d.Subdepartments)
            .FirstOrDefault();

        public Department? FindOneByName(string name)
            => Table
            .Where(d => d.Name == name)
            .Include(d => d.Subdepartments)
            .FirstOrDefault();
    }
}

using DAL.Repositories.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Department? FindOneByName(string? name);
        Task<List<Department>> FindAllAsNoTrackingAsync(string? name);
        Task<List<Department>> FindAllAsNoTrackingIncludeAsync();
        Task<List<string>> FindAllDepartmentNamesAsync();
        Task<Department> FindSingleByNameNoIncludeAsync(string name);
        Task<Department> FindSingleByNameWithIncludeAsync(string name);
        IQueryable<Department>? FindAllWithMatchingNames(IList<string> departmentsNames);
        IEnumerable<Department> FindAllMatchingSubdepartments(Department department);
        Task<Department?> FindFirstOrDefaultByNameAsync(string name);
        bool DepartmentExists(string name);
    }
}

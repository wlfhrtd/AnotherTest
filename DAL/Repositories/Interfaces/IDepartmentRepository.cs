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
    }
}

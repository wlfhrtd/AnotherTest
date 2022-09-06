using Dal.EfStructures;
using DAL.EfStructures;
using Domain.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace MVC.Services
{
    public class SyncFromFile : ISyncFromFile
    {
        private string projectMVCRootPath;
        private string filePath;
        private ApplicationDbContext applicationDbContext;

        public SyncFromFile()//[FromServices] ApplicationDbContext applicationDbContext)
        {
            //this.applicationDbContext = applicationDbContext;
            applicationDbContext = new ApplicationDbContextFactory().CreateDbContext(new string[0]);
            projectMVCRootPath = Directory.GetCurrentDirectory();
            filePath = projectMVCRootPath + Path.DirectorySeparatorChar + "departments.json";
        }

        public async Task<int> Sync()
        {
            string json = await File.ReadAllTextAsync(filePath);

            IEnumerable<Department> departmentsFromFile = JsonConvert.DeserializeObject<IEnumerable<Department>>(json);

            await Process(departmentsFromFile); // returned string is not used

            int saveChangesResult = await applicationDbContext.SaveChangesAsync();
            await applicationDbContext.DisposeAsync();

            return saveChangesResult;
        }

        private async Task<string> Process(IEnumerable<Department> departmentsFromFile)
        {
            List<Department> departmentsForInsert = new();
            List<Department> departmentsForUpdate = new();
            // fill transaction
            for (int i = 0; i < departmentsFromFile.Count(); i++)
            {
                StackIteration(departmentsFromFile.ToArray()[i],
                               ref departmentsForInsert, ref departmentsForUpdate);
            }
            // track Inserts
            for (int i = 0; i < departmentsForInsert.Count; i++)
            {
                await applicationDbContext.AddAsync(departmentsForInsert[i]);
            }
            // track Updates
            foreach (var item in departmentsForUpdate)
            {
                await UpdateWithCollections(item); // returned string is not used
            }

            return $"Entities Inserted: {departmentsForInsert.Count}\n" +
                   $"Entities Updated: {departmentsForUpdate.Count}";
        }
        // TODO move this method to DepartmentRepository
        private async Task<string> UpdateWithCollections(Department departmentForUpdate)
        {
            Department departmentOriginal = await applicationDbContext.Departments
                            .Include("Subdepartments")
                            .SingleAsync(orig => orig.Name == departmentForUpdate.Name);

            var subdepartments = applicationDbContext.Departments
                    .AsEnumerable()
                    .Where(d => departmentForUpdate.Subdepartments.Any(include => include.Name == d.Name));

            departmentOriginal.Subdepartments = subdepartments.ToList();

            applicationDbContext.Update(departmentOriginal);

            return $"Collection update diff count:" +
                   $" {departmentForUpdate.Subdepartments.Count - departmentOriginal.Subdepartments.Count}";
        }

        private void StackIteration(Department department,
                ref List<Department> departmentsForInsert, ref List<Department> departmentsForUpdate)
        {
            Stack<Department> departmentsStack = new(10);

            departmentsStack.Push(department);

            while (departmentsStack.Count > 0)
            {
                Department current = departmentsStack.Pop();

                if (DepartmentExists(current.Name))
                {
                    departmentsForUpdate.Add(current);
                }
                else
                {
                    departmentsForInsert.Add(current);
                }

                if (current.Subdepartments.Count != 0)
                {
                    foreach (Department subdepartment in current.Subdepartments)
                    {
                        departmentsStack.Push(subdepartment);
                    }
                }
            }
        }
        // TODO move this method to DepartmentRepository
        private bool DepartmentExists(string name)
        {
            return (applicationDbContext.Departments?.Any(e => e.Name == name)).GetValueOrDefault();
        } 
    }
}

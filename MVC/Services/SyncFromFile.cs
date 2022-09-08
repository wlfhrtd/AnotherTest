using Dal.EfStructures;
using DAL.EfStructures;
using DAL.Repositories.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MVC.Services
{
    public class SyncFromFile : ISyncFromFile
    {
        private string filePath;
        private IDepartmentRepository departmentRepository;

        public SyncFromFile([FromServices] IDepartmentRepository departmentRepository,
                            [FromServices] IFileManager fileManager)
        {
            this.departmentRepository = departmentRepository;
            filePath = fileManager.FilePath;
        }


        public async Task<int> Sync()
        {
            string json = await File.ReadAllTextAsync(filePath);

            IEnumerable<Department> departmentsFromFile = JsonConvert.DeserializeObject<IEnumerable<Department>>(json);

            await Process(departmentsFromFile); // returned string is not used

            int saveChangesResult = await departmentRepository.SaveChangesAsync();

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
            if (departmentsForInsert.Count != 0)
            {
                for (int i = 0; i < departmentsForInsert.Count; i++)
                {
                    await departmentRepository.AddAsync(departmentsForInsert[i], false);
                }
                // extra call is required to manage relations; otherwise relations info for new entities is lost
                await departmentRepository.SaveChangesAsync();
            }
            // track Updates
            foreach (var item in departmentsForUpdate)
            {
                await UpdateWithCollections(item); // returned string is not used
            }

            return $"Entities Inserted: {departmentsForInsert.Count}\n" +
                   $"Entities Updated: {departmentsForUpdate.Count}";
        }
        // should be in repository for re-usage (e.g. edit functionality in monitoring view)
        private async Task<string> UpdateWithCollections(Department departmentForUpdate)
        {
            Department departmentOriginal = await departmentRepository
                .FindSingleByNameWithIncludeAsync(departmentForUpdate.Name);

            var subdepartments = departmentRepository.FindAllMatchingSubdepartments(departmentForUpdate);

            departmentOriginal.Subdepartments = subdepartments.ToList();

            departmentRepository.Update(departmentOriginal, false);

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

                if (departmentRepository.DepartmentExists(current.Name))
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
    }
}

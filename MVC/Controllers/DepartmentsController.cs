using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.EfStructures;
using Domain.Models;
using DAL.Repositories.Interfaces;
using MVC.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using MVC.Services;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class DepartmentsController : Controller
    {
        [Route("/[controller]/{searchString?}")]
        [Route("/[controller]/[action]/{searchString?}")]
        public async Task<IActionResult> Index(string? searchString,
                                              [FromServices] IDepartmentRepository departmentRepository)
        {
            ViewData["CurrentFilter"] = searchString;

            return View(await departmentRepository.FindAllAsNoTrackingAsync(searchString));
        }

        [HttpGet("{name?}")]
        public async Task<IActionResult> Details(string? name,
                                                [FromServices] IDepartmentRepository departmentRepository)
        {
            if (name == null)
            {
                return NotFound();
            }

            var department = await departmentRepository.FindOneByNameAsync(name);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Department department,
                                                [FromServices] IDepartmentRepository departmentRepository)
        {
            if (ModelState.IsValid)
            {
                await departmentRepository.AddAsync(department);

                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        [HttpGet("{name?}")]
        public async Task<IActionResult> Edit(string? name,
                                             [FromServices] IDepartmentRepository departmentRepository)
        {
            if (name == null)
            {
                return NotFound();
            }

            var department = await departmentRepository.FindOneByNameAsync(name);

            if (department == null)
            {
                return NotFound();
            }

            List<string> allDepartments = await departmentRepository.FindAllDepartmentNamesAsync();

            DepartmentViewModel viewModel = new(department, ref allDepartments);

            return View(viewModel);
        }

        [HttpPost("{name}"), ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string name,
                                             [FromForm] DepartmentViewModel viewModel,
                                             [FromServices] IDepartmentRepository departmentRepository)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Department departmentForUpdate;

                    if (name != viewModel.Department.Name)
                    {
                        // Department.Name is PK; can not modify PK;
                        // should create new record, move references, remove old record
                        departmentForUpdate = new(viewModel.Department.Name);
                        departmentRepository.SetEntryStateAdded(departmentForUpdate);

                        await departmentRepository.SaveChangesAsync();

                        Department departmentForDelete =
                            await departmentRepository.FindSingleByNameNoIncludeAsync(name);
                        departmentRepository.SetEntryStateDeleted(departmentForDelete);
                    }
                    else
                    {
                        // Department.Name has not been changed;
                        // regular update
                        departmentForUpdate = await departmentRepository.FindSingleByNameWithIncludeAsync(name);
                    }
                    
                    var subdepartments = departmentRepository
                        .FindAllWithMatchingNames(viewModel.ConnectedSubdepartmentsNames);

                    departmentForUpdate.Subdepartments = subdepartments.ToList();

                    await departmentRepository.UpdateAsync(departmentForUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!departmentRepository.DepartmentExists(viewModel.Department.Name))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpGet("{name?}")]
        public async Task<IActionResult> Delete(string? name,
                                               [FromServices] IDepartmentRepository departmentRepository)
        {
            if (name == null)
            {
                return NotFound();
            }

            var department = departmentRepository.FindFirstOrDefaultByNameAsync(name);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost("{name}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string name,
                                         [FromServices] IDepartmentRepository departmentRepository)
        {
            // include Subdepartments to update set null DepartmentMain
            var department = await departmentRepository.FindOneByNameAsync(name);

            if (department != null)
            {
                await departmentRepository.RemoveAsync(department);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Monitoring([FromServices] IDepartmentRepository departmentRepository)
        {
            return View(await departmentRepository.FindAllAsNoTrackingIncludeAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Sync([FromServices] ISyncFromFile syncFromFile)
        {
            await syncFromFile.Sync();

            Thread.Sleep(2000);

            return RedirectToAction(nameof(Monitoring));
        }
    }
}

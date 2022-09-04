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

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/[controller]/{searchString?}")]
        [Route("/[controller]/[action]/{searchString?}")]
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var departments = from d in _context.Departments
                              select d;

            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments
                    .Where(d => d.Name.Contains(searchString)); // IQueryable database implementation ~ case-sensitive as default
            }

            return View(await departments.AsNoTracking().ToListAsync());
        }

        [HttpGet("{name?}")]
        public async Task<IActionResult> Details(string name)
        {
            if (name == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Name == name);
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

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [HttpGet("{name?}")]
        public async Task<IActionResult> Edit(string? name)
        {
            if (name == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Where(d => d.Name == name)
                .Include("Subdepartments")
                .SingleOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            List<string> allDepartments = await (from d in _context.Departments
                                                 select d.Name).ToListAsync();

            DepartmentViewModel viewModel = new(department, ref allDepartments);

            return View(viewModel);
        }

        [HttpPost("{name}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string name, [FromForm] DepartmentViewModel viewModel)
        {
            if (name != viewModel.Department.Name)
            {
                return NotFound();
            }

            //if (!ModelState.IsValid)
            //{
            //    List<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            //    foreach (var error in errors)
            //    {
            //        Console.WriteLine(error.ErrorMessage);
            //    }
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    var department = await _context.Departments
                        .Include("Subdepartments")
                        .SingleAsync(m => m.Name == name);

                    department.Name = viewModel.Department.Name;

                    var subdepartments = _context.Departments
                        .Where(d => viewModel.ConnectedSubdepartmentsNames
                        .Any(inc => inc == d.Name));

                    department.Subdepartments = subdepartments.ToList();
                    
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(viewModel.Department.Name))
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
        public async Task<IActionResult> Delete(string? name)
        {
            if (name == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Name == name);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost("{name}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string name)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(name);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Monitoring()
        {
            return _context.Departments != null ?
                          View(await _context.Departments.Include("Subdepartments").ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
        }

        private bool DepartmentExists(string name)
        {
          return (_context.Departments?.Any(e => e.Name == name)).GetValueOrDefault();
        }
    }
}

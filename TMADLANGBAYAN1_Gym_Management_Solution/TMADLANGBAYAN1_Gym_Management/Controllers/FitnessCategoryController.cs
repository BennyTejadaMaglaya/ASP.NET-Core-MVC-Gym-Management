using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMADLANGBAYAN1_Gym_Management.CustomControllers;
using TMADLANGBAYAN1_Gym_Management.Data;
using TMADLANGBAYAN1_Gym_Management.Models;

namespace TMADLANGBAYAN1_Gym_Management.Controllers
{
    public class FitnessCategoryController : ElephantController
	{
        private readonly GymContext _context;

        public FitnessCategoryController(GymContext context)
        {
            _context = context;
        }

        // GET: FitnessCategory
        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessCategories
                .AsNoTracking()
                .ToListAsync())
                ;
        }

        // GET: FitnessCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(fc => fc.ID == id)
                ;
            if (fitnessCategory == null)
            {
                return NotFound();
            }

            return View(fitnessCategory);
        }

        // GET: FitnessCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Category")] FitnessCategory fitnessCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(fitnessCategory);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Fitness Category successfully added!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }/**/

            return View(fitnessCategory);
        }

        // GET: FitnessCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories.FindAsync(id);
            if (fitnessCategory == null)
            {
                return NotFound();
            }
            return View(fitnessCategory);
        }

        // POST: FitnessCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var FitnessCategoryToUpdate = await _context.FitnessCategories.FirstOrDefaultAsync(fc => fc.ID == id);

            if (FitnessCategoryToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<FitnessCategory>(FitnessCategoryToUpdate, "",
                fc => fc.Category))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Fitness Category successfully updated!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessCategoryExists(FitnessCategoryToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }/**/
            }
            return View(FitnessCategoryToUpdate);
        }

        // GET: FitnessCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(fc => fc.ID == id)
                ;
            if (fitnessCategory == null)
            {
                return NotFound();
            }

            return View(fitnessCategory);
        }

        // POST: FitnessCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCategory = await _context.FitnessCategories
                .FirstOrDefaultAsync(fc => fc.ID == id)
                ;

            try
            {
                if (fitnessCategory != null)
                {
                    _context.FitnessCategories.Remove(fitnessCategory);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fitness Category successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete record. This fitness category has an associated group class and cannot be deleted.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
                }
            }/**/
            return View(fitnessCategory);
        }

        private bool FitnessCategoryExists(int id)
        {
            return _context.FitnessCategories.Any(fc => fc.ID == id);
        }
    }
}

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
using TMADLANGBAYAN1_Gym_Management.Utilities;

namespace TMADLANGBAYAN1_Gym_Management.Controllers
{
    public class InstructorDocumentController : ElephantController
	{
        private readonly GymContext _context;

        public InstructorDocumentController(GymContext context)
        {
            _context = context;
        }

		// GET: InstructorDocument
		public async Task<IActionResult> Index(int? InstructorID, string? FileNameStr,
			int? page, int? pageSizeID,
			string? actionButton, string sortDirection = "asc", string sortField = "File Name")
		{
			//List of sort options.
			//NOTE: make sure this array has matching values to the column headings
			string[] sortOptions = new[] { "File Name", "Description", "Instructor" };

			//Count the number of filters applied - start by assuming no filters
			ViewData["Filtering"] = "btn-outline-secondary";
			int numberFilters = 0;
			//Then in each "test" for filtering, add to the count of Filters applied

			var instructorDocs = _context.InstructorDocuments
				.Include(id => id.Instructor)
				.OrderBy(id => id.FileName.ToLower())
				.AsNoTracking();

			PopulateDropDownLists();

			// Filters
			if (InstructorID.HasValue)
			{
				instructorDocs = instructorDocs.Where(id => id.InstructorID == InstructorID);
				numberFilters++;
			}
			if (!string.IsNullOrEmpty(FileNameStr))
			{
				instructorDocs = instructorDocs.Where(id => id.FileName.ToLower().Contains(FileNameStr.ToLower()));
				numberFilters++;
			}

			//Give feedback about the state of the filters
			if (numberFilters != 0)
			{
				//Toggle the Open/Closed state of the collapse depending on if we are filtering
				ViewData["Filtering"] = " btn-danger";
				//Show how many filters have been applied
				ViewData["numberFilters"] = "(" + numberFilters.ToString()
					+ " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
				//Keep the Bootstrap collapse open
				@ViewData["ShowFilter"] = " show";
			}
			//Before we sort, see if we have called for a change of filtering or sorting
			if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
			{
				page = 1;//Reset page to start

				if (sortOptions.Contains(actionButton))//Change of sort is requested
				{
					if (actionButton == sortField) //Reverse order on same field
					{
						sortDirection = sortDirection == "asc" ? "desc" : "asc";
					}
					sortField = actionButton;//Sort by the button clicked
				}
			}

			//Now we know which field and direction to sort by
			if (sortField == "Description")
			{
				if (sortDirection == "asc")
				{
					instructorDocs = instructorDocs
						.OrderBy(i => i.Description);
				}
				else
				{
					instructorDocs = instructorDocs
						.OrderBy(i => i.Description);
				}
			}
			else if (sortField == "Instructor")
			{
				if (sortDirection == "asc")
				{
					instructorDocs = instructorDocs
						.OrderBy(i => i.Instructor.LastName)
						.ThenBy(i => i.Instructor.FirstName)
						.ThenBy(i => i.Instructor.MiddleName);
				}
				else
				{
					instructorDocs = instructorDocs
						.OrderByDescending(i => i.Instructor.LastName)
						.ThenBy(i => i.Instructor.FirstName)
						.ThenBy(i => i.Instructor.MiddleName);
				}
			}
			else //Sorting by FileName
			{
				if (sortDirection == "asc")
				{
					instructorDocs = instructorDocs
						.OrderBy(i => i.FileName);
				}
				else
				{
					instructorDocs = instructorDocs
						.OrderByDescending(i => i.FileName);
				}
			}
			//Set sort for next time
			ViewData["sortField"] = sortField;
			ViewData["sortDirection"] = sortDirection;

			// Handle Paging
			int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
			ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
			var pagedData = await PaginatedList<InstructorDocument>.CreateAsync(instructorDocs.AsNoTracking(), page ?? 1, pageSize);

			return View(pagedData);
		}

		// GET: InstructorDocument/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var instructorDocument = await _context.InstructorDocuments
				.Include(i => i.Instructor)
				.FirstOrDefaultAsync(m => m.ID == id);
			if (instructorDocument == null)
			{
				return NotFound();
			}

			return View(instructorDocument);
		}

		// GET: InstructorDocument/Create
		public IActionResult Create()
		{
			PopulateDropDownLists();
			return View();
		}

		// POST: InstructorDocument/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("InstructorID,ID,FileName,MimeType")] InstructorDocument instructorDocument)
		{
			try
			{
				if (ModelState.IsValid)
				{
					_context.Add(instructorDocument);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				PopulateDropDownLists();
				return View(instructorDocument);
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "Unable to save changes. " +
					"Try again, and if the problem persists, see your system administrator.");
			}
			return View(instructorDocument);
		}

		// GET: InstructorDocument/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var instructorDocument = await _context.InstructorDocuments.FindAsync(id);
			if (instructorDocument == null)
			{
				return NotFound();
			}
			PopulateDropDownLists();
			return View(instructorDocument);
		}

		// POST: InstructorDocument/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id)
		{
			var instDocToUpdate = await _context.InstructorDocuments.FindAsync(id);

			if (id != instDocToUpdate.ID)
			{
				return NotFound();
			}

			if (await TryUpdateModelAsync<InstructorDocument>(instDocToUpdate, "",
				id => id.FileName, id => id.Description))
			{
				try
				{
					await _context.SaveChangesAsync();

					var returnUrl = ViewData["returnURL"]?.ToString();
					if (string.IsNullOrEmpty(returnUrl))
					{
						return RedirectToAction(nameof(Index));
					}
					return Redirect(returnUrl);
				}
				catch (DbUpdateConcurrencyException ex)
				{
					if (!InstructorDocumentExists(instDocToUpdate.ID))
					{
						return NotFound();
					}
					else
					{
						ModelState.AddModelError("", $"Unable to save changes. " +
							$"Try again, and if the problem persists, see your system administrator. " +
							$"Db Update Concurrency Exception: {ex.GetBaseException().Message}");
					}
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", $"Unable to save changes. " +
						$"Try again, and if the problem persists, see your system administrator. {ex.GetBaseException().Message}");
				}
			}
			PopulateDropDownLists();
			return View(instDocToUpdate);
		}

		// GET: InstructorDocument/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var instructorDocument = await _context.InstructorDocuments
				.Include(i => i.Instructor)
				.FirstOrDefaultAsync(m => m.ID == id);
			if (instructorDocument == null)
			{
				return NotFound();
			}
			return View(instructorDocument);
		}

		// POST: InstructorDocument/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var instructorDocument = await _context.InstructorDocuments.FindAsync(id);
			try
			{
				if (instructorDocument != null)
				{
					_context.InstructorDocuments.Remove(instructorDocument);
				}
				await _context.SaveChangesAsync();
				var returnUrl = ViewData["returnURL"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
				{
					return RedirectToAction(nameof(Index));
				}
				return Redirect(returnUrl);
			}
			catch (DbUpdateException)
			{
				ModelState.AddModelError("", "Unable to delete file. " +
						"Try again, and if the problem persists contact your system administrator.");
			}
			return View(instructorDocument);
		}

		private SelectList InstructorSelectList(int? selectedId)
		{
			return new SelectList(_context.Instructors
				.OrderBy(i => i.LastName)
				.ThenBy(i => i.FirstName)
				.ThenBy(i => i.MiddleName), "ID", "FormalName", selectedId);
		}

		private void PopulateDropDownLists(GroupClass groupClass = null)
		{
			ViewData["InstructorID"] = InstructorSelectList(groupClass?.InstructorID);
		}

		private bool InstructorDocumentExists(int id)
        {
            return _context.InstructorDocuments.Any(e => e.ID == id);
        }
    }
}

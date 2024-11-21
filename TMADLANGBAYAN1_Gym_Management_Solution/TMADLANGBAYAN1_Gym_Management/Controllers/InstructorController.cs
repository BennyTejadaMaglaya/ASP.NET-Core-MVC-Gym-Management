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
    public class InstructorController : ElephantController
	{
        private readonly GymContext _context;

        public InstructorController(GymContext context)
        {
            _context = context;
        }

        // GET: Instructor
        public async Task<IActionResult> Index(string? SearchNameString, string? SearchNumberString,
			int? page, int? pageSizeID,
			string? actionButton, string sortDirection = "asc", string sortField = "Name")
        {
			//List of sort options.
			//NOTE: make sure this array has matching values to the column headings
			string[] sortOptions = new[] { "Name", "Seniority", "Phone Number", "Email Address" };

			//Count the number of filters applied - start by assuming no filters
			ViewData["Filtering"] = "btn-outline-secondary";
			int numberFilters = 0;
			//Then in each "test" for filtering, add to the count of Filters applied

			var gymContext = _context.Instructors
                .Include(d => d.InstructorDocuments)
                .AsNoTracking()
				;

			//Add as many filters as needed
			if (!String.IsNullOrEmpty(SearchNameString))
			{
				gymContext = gymContext.Where(i => i.LastName.ToUpper().Contains(SearchNameString.ToUpper())
									   || i.FirstName.ToUpper().Contains(SearchNameString.ToUpper()));
				numberFilters++;
			}
			if (!String.IsNullOrEmpty(SearchNumberString))
			{
				gymContext = gymContext.Where(i => i.Phone.Contains(SearchNumberString));
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
			if (sortField == "Seniority")
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderByDescending(i => i.HireDate);
				}
				else
				{
					gymContext = gymContext
						.OrderBy(i => i.HireDate);
				}
			}
			else if (sortField == "Phone Number")
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderBy(i => i.Phone)
						.ThenBy(i => i.LastName)
						.ThenBy(i => i.FirstName)
						.ThenBy(i => i.MiddleName);
				}
				else
				{
					gymContext = gymContext
						.OrderByDescending(i => i.Phone)
						.ThenBy(i => i.LastName)
						.ThenBy(i => i.FirstName)
						.ThenBy(i => i.MiddleName);
				}
			}
			else if (sortField == "Email Address")
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderBy(i => i.Email)
						.ThenBy(i => i.LastName)
						.ThenBy(i => i.FirstName)
						.ThenBy(i => i.MiddleName);
				}
				else
				{
					gymContext = gymContext
						.OrderByDescending(i => i.Email)
						.ThenBy(i => i.LastName)
						.ThenBy(i => i.FirstName)
						.ThenBy(i => i.MiddleName);
				}
			}
			else //Sorting by Instructor Name
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderBy(i => i.LastName)
						.ThenBy(i => i.FirstName)
						.ThenBy(i => i.MiddleName);
				}
				else
				{
					gymContext = gymContext
						.OrderByDescending(i => i.LastName)
						.ThenByDescending(i => i.FirstName)
						.ThenByDescending(i => i.MiddleName);
				}
			}
			//Set sort for next time
			ViewData["sortField"] = sortField;
			ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Instructor>.CreateAsync(gymContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
		}

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.GroupClasses)
                    .ThenInclude(gc => gc.FitnessCategory)
                .Include(i => i.GroupClasses)
                    .ThenInclude(gc => gc.ClassTime)
                .Include(i => i.InstructorDocuments)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id)
                ;
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,HireDate,Phone,Email,IsActive")] Instructor instructor, List<IFormFile> theFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddDocumentsAsync(instructor, theFiles);
                    _context.Add(instructor);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instructor successfully added!";
					//return RedirectToAction(nameof(Index));
					//Instead of going back to the Index, why not show the revised
					//version in full detail?
					return RedirectToAction("Details", new { instructor.ID });
				}
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Instructors.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. An instructor with this email already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }/**/

            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.InstructorDocuments)
                .FirstOrDefaultAsync(i => i.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<IFormFile> theFiles)
        {
            var InstructorToUpdate = await _context.Instructors
                .Include(i => i.InstructorDocuments)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (InstructorToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Instructor>(InstructorToUpdate, "",
                i => i.FirstName, i => i.MiddleName, i => i.LastName, i => i.HireDate, i => i.Phone, i => i.Email, i => i.IsActive))
            {
                try
                {
                    await AddDocumentsAsync(InstructorToUpdate, theFiles);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instructor successfully updated!";
					//return RedirectToAction(nameof(Index));
					//Instead of going back to the Index, why not show the revised
					//version in full detail?
					return RedirectToAction("Details", new { InstructorToUpdate.ID });
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(InstructorToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Instructors.Email"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes. An instructor with this Email already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }/**/
            }
            return View(InstructorToUpdate);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.InstructorDocuments)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id)
                ;
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.InstructorDocuments)
                .FirstOrDefaultAsync(i => i.ID == id);
            ;

            try
            {
                if (instructor != null)
                {
                    _context.Instructors.Remove(instructor);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor successfully deleted!";
				var returnUrl = ViewData["returnURL"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
				{
					return RedirectToAction(nameof(Index));
				}
				return Redirect(returnUrl);
			}
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete record. This instructor has an associated group class and cannot be deleted.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
                }
            }/**/
            return View(instructor);
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(u => u.FileContent)
                .Where(u => u.ID == id)
                .FirstOrDefaultAsync();

            if (theFile?.FileContent?.Content == null || theFile.MimeType == null)
            {
                return new FileContentResult(Array.Empty<byte>(), "application/octet-stream");
            }
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private async Task AddDocumentsAsync(Instructor instructor, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    // Note: you could filter for mime types if you only want to allow
                    // certain types of files. I am allowing everything.
                    if (!(fileName == "" || fileLength == 0)) // Looks like we have a file!
                    {
                        InstructorDocument d = new InstructorDocument();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            d.FileContent.Content = memoryStream.ToArray();
                        }
                        d.MimeType = mimeType;
                        d.FileName = fileName;
                        instructor.InstructorDocuments.Add(d);
                    };
                }
            }
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(i => i.ID == id);
        }
    }
}

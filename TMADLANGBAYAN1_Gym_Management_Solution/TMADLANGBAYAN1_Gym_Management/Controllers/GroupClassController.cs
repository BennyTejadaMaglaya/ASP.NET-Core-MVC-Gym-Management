using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TMADLANGBAYAN1_Gym_Management.Data;
using TMADLANGBAYAN1_Gym_Management.Models;
using TMADLANGBAYAN1_Gym_Management.ViewModels;
using TMADLANGBAYAN1_Gym_Management.Utilities;
using TMADLANGBAYAN1_Gym_Management.CustomControllers;

namespace TMADLANGBAYAN1_Gym_Management.Controllers
{
    public class GroupClassController : ElephantController
	{
        private readonly GymContext _context;

        public GroupClassController(GymContext context)
        {
            _context = context;
        }

        // GET: GroupClass
        public async Task<IActionResult> Index(string? DayFilter, int? FitnessCategoryID, int? ClassTimeID, string? SearchString, int? InstructorID,
			int? page, int? pageSizeID,
			string? actionButton, string sortDirection = "asc", string sortField = "Instructor")
        {
			//List of sort options.
			//NOTE: make sure this array has matching values to the column headings
			string[] sortOptions = new[] { "Instructor", "Fitness Category" };

			//Count the number of filters applied - start by assuming no filters
			ViewData["Filtering"] = "btn-outline-secondary";
			int numberFilters = 0;
			//Then in each "test" for filtering, add to the count of Filters applied

			ViewData["FitnessCategoryID"] = new SelectList(_context
				.FitnessCategories
				.OrderBy(fc => fc.Category), "ID", "Category");
			ViewData["ClassTimeID"] = new SelectList(_context
				.ClassTimes
				.OrderBy(ct => ct.StartTime), "ID", "StartTime");
			PopulateDropDownLists();

			//SelectList for the Coverage Enum
			if (Enum.TryParse(DayFilter, out EnumDayOfWeek selectedDay))
            {
                ViewBag.DaySelectList = EnumDayOfWeek.Monday.ToSelectList(selectedDay);
            }
            else
            {
                ViewBag.DaySelectList = EnumDayOfWeek.Monday.ToSelectList(null);
            }

			//Start with Includes but make sure your expression returns an
			//IQueryable<GroupClass> so we can add filter and sort 
			//options later.
			var gymContext = _context.GroupClasses
                .Include(gc => gc.ClassTime)
                .Include(gc => gc.FitnessCategory)
                .Include(gc => gc.Instructor)
                .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
                .AsNoTracking()
                ;

			//Add as many filters as needed
			if (!String.IsNullOrEmpty(DayFilter))
			{
				gymContext = gymContext.Where(gc => gc.DOW == selectedDay);
				numberFilters++;
			}
			if (FitnessCategoryID.HasValue)
			{
				gymContext = gymContext.Where(gc => gc.FitnessCategoryID == FitnessCategoryID);
				numberFilters++;
			}
			if (ClassTimeID.HasValue)
			{
				gymContext = gymContext.Where(gc => gc.ClassTimeID == ClassTimeID);
				numberFilters++;
			}
			if (!String.IsNullOrEmpty(SearchString))
			{
				gymContext = gymContext.Where(gc => gc.Description.ToUpper().Contains(SearchString.ToUpper()));
				numberFilters++;
			}
			if (InstructorID.HasValue)
			{
				gymContext = gymContext.Where(gc => gc.InstructorID == InstructorID);
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
			if (sortField == "Fitness Category")
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderBy(gc => gc.FitnessCategory.Category)
						.ThenBy(gc => gc.Instructor.LastName)
						.ThenBy(gc => gc.Instructor.FirstName)
						.ThenBy(gc => gc.Instructor.MiddleName);
				}
				else
				{
					gymContext = gymContext
						.OrderByDescending(gc => gc.FitnessCategory.Category)
						.ThenBy(gc => gc.Instructor.LastName)
						.ThenBy(gc => gc.Instructor.FirstName)
						.ThenBy(gc => gc.Instructor.MiddleName);
				}
			}
			else //Sorting by Instructor Name
			{
				if (sortDirection == "asc")
				{
					gymContext = gymContext
						.OrderBy(gc => gc.Instructor.LastName)
						.ThenBy(gc => gc.Instructor.FirstName)
						.ThenBy(gc => gc.Instructor.MiddleName);
				}
				else
				{
					gymContext = gymContext
						.OrderByDescending(gc => gc.Instructor.LastName)
						.ThenByDescending(gc => gc.Instructor.FirstName)
						.ThenByDescending(gc => gc.Instructor.MiddleName);
				}
			}
			//Set sort for next time
			ViewData["sortField"] = sortField;
			ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<GroupClass>.CreateAsync(gymContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
		}

        // GET: GroupClass/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses
                .Include(gc => gc.ClassTime)
                .Include(gc => gc.FitnessCategory)
                .Include(gc => gc.Instructor)
                .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(gc => gc.ID == id)
                ;
            if (groupClass == null)
            {
                return NotFound();
            }

            return View(groupClass);
        }

        // GET: GroupClass/Create
        public IActionResult Create()
        {
            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category");
            ViewData["ClassTimeID"] = new SelectList(_context.ClassTimes, "ID", "StartTime");
            PopulateDropDownLists();

            GroupClass gc = new GroupClass();
            PopulateAssignedClientData(gc);

            return View(gc);
        }

        // POST: GroupClass/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,DOW,FitnessCategoryID,ClassTimeID,InstructorID")] GroupClass groupClass, string[] selectedOptions)
        {
            try
            {
                UpdateEnrollments(selectedOptions, groupClass);
                if (ModelState.IsValid)
                {
                    _context.Add(groupClass);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Group Class successfully added!";
					//return RedirectToAction(nameof(Index));
					//Instead of going back to the Index, why not show the revised
					//version in full detail?
					return RedirectToAction("Details", new { groupClass.ID });
				}
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to save changes. An instructor cannot be running two classes at the same time.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }/**/

            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", groupClass.FitnessCategoryID);
            ViewData["ClassTimeID"] = new SelectList(_context.ClassTimes, "ID", "StartTime", groupClass.ClassTimeID);
            PopulateDropDownLists(groupClass);

            //Validation Error so give the user another chance.
            PopulateAssignedClientData(groupClass);

            return View(groupClass);
        }

        // GET: GroupClass/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses
               .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
               .FirstOrDefaultAsync(gc => gc.ID == id)
               ;
            if (groupClass == null)
            {
                return NotFound();
            }

            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", groupClass.FitnessCategoryID);
            ViewData["ClassTimeID"] = new SelectList(_context.ClassTimes, "ID", "StartTime", groupClass.ClassTimeID);
            PopulateDropDownLists(groupClass);
            PopulateAssignedClientData(groupClass);

            return View(groupClass);
        }

        // POST: GroupClass/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, Byte[] RowVersion)
        {
            var GroupClassToUpdate = await _context.GroupClasses
                .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
                .FirstOrDefaultAsync(gc => gc.ID == id);

            if (GroupClassToUpdate == null)
            {
                return NotFound();
            }

			//Put the original RowVersion value in the OriginalValues collection for the entity
			_context.Entry(GroupClassToUpdate).Property("RowVersion").OriginalValue = RowVersion;

			//Update the Enrollments
			UpdateEnrollments(selectedOptions, GroupClassToUpdate);

            if (await TryUpdateModelAsync<GroupClass>(GroupClassToUpdate, "",
                gc => gc.Description, gc => gc.DOW, gc => gc.FitnessCategoryID, gc => gc.ClassTimeID, gc => gc.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Group Class successfully updated!";
                    //return RedirectToAction(nameof(Index));
                    //Instead of going back to the Index, why not show the revised
                    //version in full detail?
                    return RedirectToAction("Details", new { GroupClassToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
				catch (DbUpdateConcurrencyException ex)// Added for concurrency
				{
					var exceptionEntry = ex.Entries.Single();
					var clientValues = (GroupClass)exceptionEntry.Entity;
					var databaseEntry = exceptionEntry.GetDatabaseValues();
					if (databaseEntry == null)
					{
						ModelState.AddModelError("",
							"Unable to save changes. The Group Class was deleted by another user.");
					}
					else
					{
						var databaseValues = (GroupClass)databaseEntry.ToObject();
						if (databaseValues.Description != clientValues.Description)
							ModelState.AddModelError("Description", "Current value: "
								+ databaseValues.Description);
						if (databaseValues.DOW != clientValues.DOW)
							ModelState.AddModelError("DOW", "Current value: "
								+ databaseValues.DOW);
						
						//For the foreign key, we need to go to the database to get the information to show
						if (databaseValues.FitnessCategoryID != clientValues.FitnessCategoryID)
						{
							FitnessCategory? databaseFitnessCategory = await _context.FitnessCategories.FirstOrDefaultAsync(i => i.ID == databaseValues.FitnessCategoryID);
							ModelState.AddModelError("FitnessCategoryID", $"Current value: {databaseFitnessCategory?.Category}");
						}
						if (databaseValues.ClassTimeID != clientValues.ClassTimeID)
						{
							ClassTime? databaseClassTime = await _context.ClassTimes.FirstOrDefaultAsync(i => i.ID == databaseValues.ClassTimeID);
							ModelState.AddModelError("ClassTimeID", $"Current value: {databaseClassTime?.StartTime}");
						}
						if (databaseValues.InstructorID != clientValues.InstructorID)
						{
							Instructor? databaseInstructor = await _context.Instructors.FirstOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
							ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor?.FormalName}");
						}
						
						ModelState.AddModelError(string.Empty, "The record you attempted to edit "
								+ "was modified by another user after you received your values. The "
								+ "edit operation was canceled and the current values in the database "
								+ "have been displayed. If you still want to save your version of this record, click "
								+ "the Save button again. Otherwise click the 'Back to Group Class List' hyperlink.");

						//Final steps before redisplaying: Update RowVersion from the Database
						//and remove the RowVersion error from the ModelState
						GroupClassToUpdate.RowVersion = databaseValues.RowVersion ?? Array.Empty<byte>();
						ModelState.Remove("RowVersion");
					}
				}
				catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. The Instructor or Fitness Category selected is invalid or does not exist.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }/**/
            }

            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", GroupClassToUpdate.FitnessCategoryID);
            ViewData["ClassTimeID"] = new SelectList(_context.ClassTimes, "ID", "StartTime", GroupClassToUpdate.ClassTimeID);
            PopulateDropDownLists(GroupClassToUpdate);

            //Validation Error so give the user another chance.
            PopulateAssignedClientData(GroupClassToUpdate);

            return View(GroupClassToUpdate);
        }

        // GET: GroupClass/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupClass = await _context.GroupClasses
                .Include(gc => gc.ClassTime)
                .Include(gc => gc.FitnessCategory)
                .Include(gc => gc.Instructor)
                .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(gc => gc.ID == id)
                ;
            if (groupClass == null)
            {
                return NotFound();
            }

            return View(groupClass);
        }

        // POST: GroupClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupClass = await _context.GroupClasses
                .Include(gc => gc.ClassTime)
                .Include(gc => gc.FitnessCategory)
                .Include(gc => gc.Instructor)
                .Include(gc => gc.Enrollments).ThenInclude(e => e.Client)
                .FirstOrDefaultAsync(gc => gc.ID == id)
                ;

            try
            {
                if (groupClass != null)
                {
                    _context.GroupClasses.Remove(groupClass);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Group Class successfully deleted!";
				var returnUrl = ViewData["returnURL"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
				{
					return RedirectToAction(nameof(Index));
				}
				return Redirect(returnUrl);
			}
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }/**/
            return View(groupClass);
        }

        private void PopulateDropDownLists(GroupClass? gc = null)
        {
            /*BONUS*/
            IQueryable<Instructor> iQuery;

            if (gc == null) /*creating a new GroupClass*/
            {
                iQuery = from i in _context.Instructors
                         where i.IsActive
                         orderby i.LastName, i.FirstName, i.MiddleName
                         select i;
            }
            else /*editing a GroupClass*/
            {
                iQuery = from i in _context.Instructors
                         orderby i.LastName, i.FirstName, i.MiddleName
                         select i;
            }

            ViewData["InstructorID"] = new SelectList(iQuery, "ID", "FormalName", gc?.InstructorID);
        }

        private void PopulateAssignedClientData(GroupClass gc)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Clients;
            var currentOptionsHS = new HashSet<int>(gc.Enrollments.Select(e => e.ClientID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var c in allOptions)
            {
                if (currentOptionsHS.Contains(c.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = c.ID,
                        DisplayText = c.FormalName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = c.ID,
                        DisplayText = c.FormalName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(c => c.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(c => c.DisplayText), "ID", "DisplayText");
        }
        private void UpdateEnrollments(string[] selectedOptions, GroupClass GroupClassToUpdate)
        {
            if (selectedOptions == null)
            {
                GroupClassToUpdate.Enrollments = new List<Enrollment>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(GroupClassToUpdate.Enrollments.Select(b => b.ClientID));
            foreach (var c in _context.Clients)
            {
                if (selectedOptionsHS.Contains(c.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(c.ID))//but not currently in the GroupClass' collection - Add it!
                    {
                        GroupClassToUpdate.Enrollments.Add(new Enrollment
                        {
                            ClientID = c.ID,
                            GroupClassID = GroupClassToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(c.ID))//but is currently in the GroupClass' collection - Remove it!
                    {
                        Enrollment? specToRemove = GroupClassToUpdate.Enrollments
                            .FirstOrDefault(e => e.ClientID == c.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }

        private bool GroupClassExists(int id)
        {
            return _context.GroupClasses.Any(gc => gc.ID == id);
        }
    }
}

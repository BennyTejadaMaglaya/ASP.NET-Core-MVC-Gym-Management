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
    public class ClientWorkoutController : ElephantController
	{
        private readonly GymContext _context;

        public ClientWorkoutController(GymContext context)
        {
            _context = context;
        }

		// GET: ClientWorkout
		public async Task<IActionResult> Index(int? ClientID, int? page, int? pageSizeID, int? InstructorID, string actionButton,
			string SearchString, string sortDirection = "desc", string sortField = "Workout")
		{
			//Get the URL with the last filter, sort and page parameters from THE CLIENTS Index View
			ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, "Client");

			if (!ClientID.HasValue)
			{
				//Go back to the proper return URL for the Clients controller
				return Redirect(ViewData["returnURL"].ToString());
			}

			PopulateDropDownLists();

			//Count the number of filters applied - start by assuming no filters
			ViewData["Filtering"] = "btn-outline-secondary";
			int numberFilters = 0;
			//Then in each "test" for filtering, add to the count of Filters applied

			//NOTE: make sure this array has matching values to the column headings
			string[] sortOptions = new[] { "Workout", "Instructor" };

			var workout = from w in _context.Workouts
						.Include(w => w.Instructor)
						.Include(w => w.Client)
						.Include(w => w.WorkoutExercises)
						where w.ClientID == ClientID.GetValueOrDefault()
						select w;

			if (InstructorID.HasValue)
			{
				workout = workout.Where(w => w.InstructorID == InstructorID);
				numberFilters++;
			}
			if (!String.IsNullOrEmpty(SearchString))
			{
				workout = workout.Where(w => w.Notes.ToUpper().Contains(SearchString.ToUpper()));
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
				//@ViewData["ShowFilter"] = " show";
			}

			//Before we sort, see if we have called for a change of filtering or sorting
			if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
			{
				page = 1;//Reset back to first page when sorting or filtering

				if (sortOptions.Contains(actionButton))//Change of sort is requested
				{
					if (actionButton == sortField) //Reverse order on same field
					{
						sortDirection = sortDirection == "asc" ? "desc" : "asc";
					}
					sortField = actionButton;//Sort by the button clicked
				}
			}
			//Now we know which field and direction to sort by.
			if (sortField == "Instructor")
			{
				if (sortDirection == "asc")
				{
					workout = workout
						.OrderBy(w => w.Instructor.FormalName);
				}
				else
				{
					workout = workout
						.OrderByDescending(w => w.Instructor.FormalName);
				}
			}
			else //Workout Date
			{
				if (sortDirection == "asc")
				{
					workout = workout
						.OrderByDescending(w => w.StartTime);
				}
				else
				{
					workout = workout
						.OrderBy(w => w.StartTime);
				}
			}
			//Set sort for next time
			ViewData["sortField"] = sortField;
			ViewData["sortDirection"] = sortDirection;

			//Now get the MASTER record, the client, so it can be displayed at the top of the screen
			Client? client = await _context.Clients
				.Include(c => c.ClientThumbnail)
				.Include(c => c.Enrollments).ThenInclude(e => e.GroupClass)
				.Where(c => c.ID == ClientID.GetValueOrDefault())
				.AsNoTracking()
				.FirstOrDefaultAsync();

			ViewBag.Client = client;

			//Handle Paging
			int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
			ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

			var pagedData = await PaginatedList<Workout>.CreateAsync(workout.AsNoTracking(), page ?? 1, pageSize);

			return View(pagedData);
		}

		// GET: ClientWorkout/Create
		// GET: ClientWorkout/Add
		public IActionResult Add(int? ClientID, string ClientName)
		{
			if (!ClientID.HasValue)
			{
				return Redirect(ViewData["returnURL"].ToString());
			}

			ViewData["ClientName"] = ClientName;
			//ViewData["Duration"] = new SelectList(DurationItems, "20");
			Workout w = new Workout()
			{
				ClientID = ClientID.GetValueOrDefault(),
				//StartTime = DateUtilities.GetNextWeekday(DateTime.Today, 3)
			};
			PopulateDropDownLists();
			return View(w);
		}

		// POST: ClientWorkout/Add
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add([Bind("StartTime,EndTime,Notes,ClientID,InstructorID")] Workout workout, string ClientName, int Duration)
		{
			try
			{
				workout.EndTime = workout.StartTime.AddMinutes(Duration);
				if (ModelState.IsValid)
				{
					_context.Add(workout);
					await _context.SaveChangesAsync();
					return Redirect(ViewData["returnURL"].ToString());
				}
			}
			catch (DbUpdateException)
			{
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem " +
					"persists see your system administrator.");
			}

			PopulateDropDownLists(workout);
			ViewData["ClientName"] = ClientName;
			//ViewData["Duration"] = new SelectList(DurationItems, Duration);
			return View(workout);
		}

		// GET: ClientWorkout/Update/5
		public async Task<IActionResult> Update(int? id)
		{
			if (id == null || _context.Workouts == null)
			{
				return NotFound();
			}

			var workout = await _context.Workouts
			   .Include(w => w.Instructor)
			   .Include(w => w.Client)
			   .AsNoTracking()
			   .FirstOrDefaultAsync(w => w.ID == id);
			if (workout == null)
			{
				return NotFound();
			}

			PopulateDropDownLists(workout);
			return View(workout);

		}


		// POST: ClientWorkout/Update/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id)
		{
			var workoutToUpdate = await _context.Workouts
				.Include(w => w.Instructor)
				.Include(w => w.Client)
				.FirstOrDefaultAsync(w => w.ID == id);

			//Check that you got it or exit with a not found error
			if (workoutToUpdate == null)
			{
				return NotFound();
			}

			//Try updating it with the values posted
			if (await TryUpdateModelAsync<Workout>(workoutToUpdate, "",
				w => w.StartTime, w => w.EndTime, w => w.Notes, w => w.InstructorID))
			{
				try
				{
					_context.Update(workoutToUpdate);
					await _context.SaveChangesAsync();
					return Redirect(ViewData["returnURL"].ToString());
				}

				catch (DbUpdateConcurrencyException)
				{
					if (!WorkoutExists(workoutToUpdate.ID))
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
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem " +
						"persists see your system administrator.");
				}
			}
			PopulateDropDownLists(workoutToUpdate);
			return View(workoutToUpdate);
		}


		// GET: ClientWorkout/Remove/5
		public async Task<IActionResult> Remove(int? id)
		{
			if (id == null || _context.Workouts == null)
			{
				return NotFound();
			}

			var workout = await _context.Workouts
				.Include(w => w.Instructor)
				.Include(w => w.Client)
				.Include(w => w.WorkoutExercises)
				.AsNoTracking()
				.FirstOrDefaultAsync(w => w.ID == id);
			if (workout == null)
			{
				return NotFound();
			}
			return View(workout);
		}

		// POST: ClientWorkout/Remove/5
		[HttpPost, ActionName("Remove")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveConfirmed(int id)
		{
			var workout = await _context.Workouts
				.Include(w => w.Instructor)
				.Include(w => w.Client)
				.Include(w => w.WorkoutExercises)
				.FirstOrDefaultAsync(w => w.ID == id);

			try
			{
				_context.Workouts.Remove(workout);
				await _context.SaveChangesAsync();
				return Redirect(ViewData["returnURL"].ToString());
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem " +
					"persists see your system administrator.");
			}

			return View(workout);
		}

		private SelectList InstructorSelectList(int? id)
		{
			var dQuery = from i in _context.Instructors
						 orderby i.LastName, i.FirstName
						 select i;
			return new SelectList(dQuery, "ID", "FormalName", id);
		}
		private void PopulateDropDownLists(Workout? workout = null)
		{
			ViewData["InstructorID"] = InstructorSelectList(workout?.InstructorID);
		}

		private bool WorkoutExists(int id)
        {
            return _context.Workouts.Any(w => w.ID == id);
            return _context.Workouts.Any(w => w.ID == id);
        }
    }
}

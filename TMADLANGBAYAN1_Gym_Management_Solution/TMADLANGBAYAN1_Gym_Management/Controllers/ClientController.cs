using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class ClientController : ElephantController
	{
        private readonly GymContext _context;

        public ClientController(GymContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var gymContext = _context.Clients
                .Include(c => c.MembershipType)
                .AsNoTracking()
                ;

			//Handle Paging
			int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
			ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
			var pagedData = await PaginatedList<Client>.CreateAsync(gymContext.AsNoTracking(), page ?? 1, pageSize);

			return View(pagedData);
		}

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.MembershipType)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ID == id)
                ;
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            ViewData["MembershipTypeID"] = new SelectList(_context.MembershipTypes, "ID", "Type");
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MembershipNumber,FirstName,MiddleName,LastName,Phone,Email,DOB,PostalCode,HealthCondition,Notes,MembershipStartDate,MembershipEndDate,MembershipFee,FeePaid,MembershipTypeID")] Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client successfully added!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Clients.MembershipNumber"))
                {
                    ModelState.AddModelError("MembershipNumber", "Unable to save changes. A client with this membership number already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }/**/

            ViewData["MembershipTypeID"] = new SelectList(_context.MembershipTypes, "ID", "Type", client.MembershipTypeID);
            
            return View(client);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["MembershipTypeID"] = new SelectList(_context.MembershipTypes, "ID", "Type", client.MembershipTypeID);
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion)
        {
            var ClientToUpdate = await _context.Clients.FirstOrDefaultAsync(c => c.ID == id);

            if (ClientToUpdate == null)
            {
                return NotFound();
            }

			//Put the original RowVersion value in the OriginalValues collection for the entity
			_context.Entry(ClientToUpdate).Property("RowVersion").OriginalValue = RowVersion;

			if (await TryUpdateModelAsync<Client>(ClientToUpdate, "",
                c => c.MembershipNumber, c => c.FirstName, c => c.MiddleName, c => c.LastName, c => c.Phone, c => c.Email, c => c.DOB, c => c.PostalCode, c => c.HealthCondition, c => c.Notes, c => c.MembershipStartDate, c => c.MembershipEndDate, c => c.MembershipTypeID, c => c.MembershipFee, c => c.FeePaid))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client successfully updated!";
                    return RedirectToAction(nameof(Index));
                }
				catch (DbUpdateConcurrencyException ex)// Added for concurrency
				{
					var exceptionEntry = ex.Entries.Single();
					var clientValues = (Client)exceptionEntry.Entity;
					var databaseEntry = exceptionEntry.GetDatabaseValues();
					if (databaseEntry == null)
					{
						ModelState.AddModelError("",
							"Unable to save changes. The Client was deleted by another user.");
					}
					else
					{
						var databaseValues = (Client)databaseEntry.ToObject();
						if (databaseValues.MembershipNumber != clientValues.MembershipNumber)
							ModelState.AddModelError("MembershipNumber", "Current value: "
								+ databaseValues.MembershipNumber);
						if (databaseValues.FirstName != clientValues.FirstName)
							ModelState.AddModelError("FirstName", "Current value: "
								+ databaseValues.FirstName);
						if (databaseValues.MiddleName != clientValues.MiddleName)
							ModelState.AddModelError("MiddleName", "Current value: "
								+ databaseValues.MiddleName);
						if (databaseValues.LastName != clientValues.LastName)
							ModelState.AddModelError("LastName", "Current value: "
								+ databaseValues.LastName);
						if (databaseValues.Phone != clientValues.Phone)
							ModelState.AddModelError("Phone", "Current value: "
								+ databaseValues.PhoneFormatted);
						if (databaseValues.Email != clientValues.Email)
							ModelState.AddModelError("Email", "Current value: "
								+ databaseValues.Email);
						if (databaseValues.DOB != clientValues.DOB)
							ModelState.AddModelError("DOB", "Current value: "
								+ String.Format("{0:d}", databaseValues.DOB));
						if (databaseValues.PostalCode != clientValues.PostalCode)
							ModelState.AddModelError("PostalCode", "Current value: "
								+ databaseValues.PostalCode);
						if (databaseValues.HealthCondition != clientValues.HealthCondition)
							ModelState.AddModelError("HealthCondition", "Current value: "
								+ databaseValues.HealthCondition);
						if (databaseValues.Notes != clientValues.Notes)
							ModelState.AddModelError("Notes", "Current value: "
								+ databaseValues.Notes);
						if (databaseValues.MembershipStartDate != clientValues.MembershipStartDate)
							ModelState.AddModelError("MembershipStartDate", "Current value: "
								+ String.Format("{0:d}", databaseValues.MembershipStartDate));
						if (databaseValues.MembershipEndDate != clientValues.MembershipEndDate)
							ModelState.AddModelError("MembershipEndDate", "Current value: "
								+ String.Format("{0:d}", databaseValues.MembershipEndDate));
						if (databaseValues.MembershipFee != clientValues.MembershipFee)
							ModelState.AddModelError("MembershipFee", "Current value: "
								+ databaseValues.MembershipFee);
						if (databaseValues.FeePaid != clientValues.FeePaid)
							ModelState.AddModelError("FeePaid", "Current value: "
								+ databaseValues.FeePaid);

						//For the foreign key, we need to go to the database to get the information to show
						if (databaseValues.MembershipTypeID != clientValues.MembershipTypeID)
						{
							MembershipType? databaseMembershipType = await _context.MembershipTypes.FirstOrDefaultAsync(i => i.ID == databaseValues.MembershipTypeID);
							ModelState.AddModelError("MembershipTypeID", $"Current value: {databaseMembershipType?.Type}");
						}

						ModelState.AddModelError(string.Empty, "The record you attempted to edit "
								+ "was modified by another user after you received your values. The "
								+ "edit operation was canceled and the current values in the database "
								+ "have been displayed. If you still want to save your version of this record, click "
								+ "the Save button again. Otherwise click the 'Back to Client List' hyperlink.");
                        
						//Final steps before redisplaying: Update RowVersion from the Database
						//and remove the RowVersion error from the ModelState
						ClientToUpdate.RowVersion = databaseValues.RowVersion ?? Array.Empty<byte>();
						ModelState.Remove("RowVersion");
					}
				}
				catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Clients.MembershipNumber"))
                    {
                        ModelState.AddModelError("MembershipNumber", "Unable to save changes. A client with this membership number already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }/**/
            }
            
            ViewData["MembershipTypeID"] = new SelectList(_context.MembershipTypes, "ID", "Type", ClientToUpdate.MembershipTypeID);
            return View(ClientToUpdate);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.MembershipType)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ID == id)
                ;
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients
                .Include(c => c.MembershipType)
                .FirstOrDefaultAsync(c => c.ID == id)
                ;

            try
            {
                if (client != null)
                {
                    _context.Clients.Remove(client);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
			catch (DbUpdateException dex)
			{
				if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
				{
					ModelState.AddModelError("", "Unable to delete record. This client has an associated group class and cannot be deleted.");
				}
				else
				{
					ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
				}
			}/**/
			return View(client);
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(c => c.ID == id);
        }
    }
}

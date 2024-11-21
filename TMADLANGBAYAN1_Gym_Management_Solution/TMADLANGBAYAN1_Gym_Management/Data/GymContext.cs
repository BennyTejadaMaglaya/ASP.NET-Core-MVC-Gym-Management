/* === Teejay Madlangbayan ======== */
/* === Student Number : 4518838 === */
using Microsoft.EntityFrameworkCore;
using TMADLANGBAYAN1_Gym_Management.Models;

namespace TMADLANGBAYAN1_Gym_Management.Data
{
	public class GymContext : DbContext
	{
		//To give access to IHttpContextAccessor for Audit Data with IAuditable
		private readonly IHttpContextAccessor _httpContextAccessor;

		//Property to hold the UserName value
		public string UserName
		{
			get; private set;
		}

		public GymContext(DbContextOptions<GymContext> options, IHttpContextAccessor httpContextAccessor)
			: base(options)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			if (_httpContextAccessor.HttpContext != null)
			{
				//We have a HttpContext, but there might not be anyone Authenticated
				UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
			}
			else
			{
				//No HttpContext so seeding data
				UserName = "Seed Data";
			}
		}

		public GymContext(DbContextOptions<GymContext> options)
			: base(options)
		{
			_httpContextAccessor = null!;
			UserName = "Seed Data";
		}

		public DbSet<FitnessCategory> FitnessCategories { get; set; }
		public DbSet<Instructor> Instructors { get; set; }
		public DbSet<GroupClass> GroupClasses { get; set; }
		public DbSet<ClassTime> ClassTimes { get; set; }
		public DbSet<MembershipType> MembershipTypes { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			/*Composite*/

			modelBuilder.Entity<Enrollment>()
				.HasKey(e => new { e.ClientID, e.GroupClassID });

			/*Unique*/

			modelBuilder.Entity<Instructor>()
				.HasIndex(i => i.Email)
				.IsUnique();

			modelBuilder.Entity<Client>()
				.HasIndex(c => c.MembershipNumber)
				.IsUnique();

			modelBuilder.Entity<GroupClass>()
				.HasIndex(gc => new { gc.ClassTimeID, gc.DOW, gc.InstructorID })
				.IsUnique();

			/*Child viewpoint*/

			modelBuilder.Entity<GroupClass>()
				.HasOne<FitnessCategory>(gc => gc.FitnessCategory)
				.WithMany(fc => fc.GroupClasses)
				.HasForeignKey(gc => gc.FitnessCategoryID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<GroupClass>()
				.HasOne<Instructor>(gc => gc.Instructor)
				.WithMany(i => i.GroupClasses)
				.HasForeignKey(gc => gc.InstructorID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<GroupClass>()
				.HasOne<ClassTime>(gc => gc.ClassTime)
				.WithMany(ct => ct.GroupClasses)
				.HasForeignKey(gc => gc.ClassTimeID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Client>()
				.HasOne<MembershipType>(c => c.MembershipType)
				.WithMany(mt => mt.Clients)
				.HasForeignKey(c => c.MembershipTypeID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Enrollment>()
				.HasOne<Client>(e => e.Client)
				.WithMany(c => c.Enrollments)
				.HasForeignKey(e => e.ClientID)
				.OnDelete(DeleteBehavior.Restrict);
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			OnBeforeSaving();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
		{
			OnBeforeSaving();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void OnBeforeSaving()
		{
			var entries = ChangeTracker.Entries();
			foreach (var entry in entries)
			{
				if (entry.Entity is IAuditable trackable)
				{
					var now = DateTime.UtcNow;
					switch (entry.State)
					{
						case EntityState.Modified:
							trackable.UpdatedOn = now;
							trackable.UpdatedBy = UserName;
							break;

						case EntityState.Added:
							trackable.CreatedOn = now;
							trackable.CreatedBy = UserName;
							trackable.UpdatedOn = now;
							trackable.UpdatedBy = UserName;
							break;
					}
				}
			}
		}
	}
}

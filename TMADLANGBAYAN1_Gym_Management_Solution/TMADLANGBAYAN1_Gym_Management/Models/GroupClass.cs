using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class GroupClass : Auditable
	{
        public int ID { get; set; }

        [Display(Name = "Summary")]
        public string Summary
        {
            get
            {
                string category = FitnessCategory?.Category ?? "Unknown Category";
                string time = ClassTime?.StartTime ?? "Unknown Time";
                return $"{category} – {DOW} {time}";
            }
        }

        [Display(Name = "Description")]
        public string TruncatedDescription
        {
            get
            {
                return Description.Length <= 20 ? Description : $"{Description.Substring(0, 20)}...";
            }
        }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 200 characters.")]
        public string Description { get; set; } = "";

        [Display(Name = "Day")]
        [Required(ErrorMessage = "Day of the week is required.")]
        /*Enum (Monday-Saturday)*/
        public EnumDayOfWeek DOW { get; set; }

        /*Foreign Keys and Navigation Properties*/
        [Display(Name = "Fitness Category")]
        [Required(ErrorMessage = "Please select a fitness category.")]
        public int FitnessCategoryID { get; set; }

        [Display(Name = "Fitness Category")]
        public FitnessCategory? FitnessCategory { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Please select a time slot.")]
        public int ClassTimeID { get; set; }

        [Display(Name = "Start Time")]
        public ClassTime? ClassTime { get; set; }

        [Display(Name = "Instructor")]
        [Required(ErrorMessage = "Please select an instructor.")]
        public int InstructorID { get; set; }

        [Display(Name = "Instructor")]
        public Instructor? Instructor { get; set; }

        [Display(Name = "Clients")]
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();

		[ScaffoldColumn(false)]
		[Timestamp]
		public Byte[]? RowVersion { get; set; }//Added for concurrency
	}
}

using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class Workout : IValidatableObject
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You must enter a start date and time for the workout.")]
        [Display(Name = "Start")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Display(Name = "End")]
        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Only 200 characters for notes.")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = "";

        [Required(ErrorMessage = "You must select the Client.")]
        [Display(Name = "Client")]
        public int ClientID { get; set; }
        public Client? Client { get; set; }

        [Required(ErrorMessage = "You must select the Instructor leading the workout.")]
        [Display(Name = "Instructor")]
        public int InstructorID { get; set; }

        public Instructor? Instructor { get; set; }

        [Display(Name = "Exercises")]
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new HashSet<WorkoutExercise>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime < StartTime)
            {
                yield return new ValidationResult("Workout cannot end before it starts.", new[] { "EndTime" });
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class Exercise
    {
        public int ID { get; set; }

        [Display(Name = "Exercise")]
        [Required(ErrorMessage = "You cannot leave the exercise name blank.")]
        [StringLength(50, ErrorMessage = "Exercise name cannot be more than 50 characters long.")]
        public string Name { get; set; } = "";

        [Display(Name = "Fitness Categories")]
        public ICollection<ExerciseCategory> ExerciseCategories { get; set; } = new HashSet<ExerciseCategory>();

        [Display(Name = "Workouts")]
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new HashSet<WorkoutExercise>();
    }
}

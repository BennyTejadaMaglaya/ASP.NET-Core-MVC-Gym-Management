namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class ExerciseCategory
    {
        public int FitnessCategoryID { get; set; }
        public FitnessCategory? FitnessCategory { get; set; }

        public int ExerciseID { get; set; }
        public Exercise? Exercise { get; set; }
    }
}

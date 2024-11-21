using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class FitnessCategory
    {
        public int ID { get; set; }

        [Display(Name = "Fitness Category")]
        [Required(ErrorMessage = "Fitness category name is required.")]
        [StringLength(50, ErrorMessage = "Fitness category name is limited to 50 characters.")]
        public string Category { get; set; } = "";

        public ICollection<GroupClass> GroupClasses { get; set; } = new HashSet<GroupClass>();
    }
}

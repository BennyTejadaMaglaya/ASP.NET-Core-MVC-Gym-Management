using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class ClassTime
    {
        /*Not an Identity*/
        [Key]
        public int ID { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start time is required.")]
        [StringLength(8, ErrorMessage = "Start time is limited to 8 characters.")]
        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public string StartTime { get; set; } = "";

        public ICollection<GroupClass> GroupClasses { get; set; } = new HashSet<GroupClass>();
    }
}

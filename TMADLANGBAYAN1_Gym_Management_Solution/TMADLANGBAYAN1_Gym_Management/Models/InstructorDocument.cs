using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class InstructorDocument : UploadedFile
    {
        [Display(Name = "Instructor")]
        public int InstructorID { get; set; }

        public Instructor? Instructor { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters.")]
        public string Description { get; set; } = "";
    }
}

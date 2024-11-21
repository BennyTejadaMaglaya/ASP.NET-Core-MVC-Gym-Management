using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class MembershipType
    {
        public int ID { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Membership type is required.")]
        [StringLength(50, ErrorMessage = "Membership type is limited to 50 characters.")]
        public string Type { get; set; } = "";

        [Display(Name = "Standard Fee")]
        [Required(ErrorMessage = "Standard fee is required.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(9,2)")]
        public double StandardFee { get; set; }

        public ICollection<Client> Clients { get; set; } = new HashSet<Client>();
    }
}

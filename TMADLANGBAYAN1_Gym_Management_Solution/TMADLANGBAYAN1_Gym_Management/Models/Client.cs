using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class Client : Auditable, IValidatableObject
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string FormalName
        {
            get
            {
                return LastName + ", " + FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? "" :
                        (" " + (char?)MiddleName[0] + ".").ToUpper());
            }
        }

        [Display(Name = "Age")]
        public string? Age
        {
            get
            {
                DateTime today = DateTime.Today;

                if (DOB == null)
                {
                    return null;
                }

                /*including leap years*/
                DateTime birthDate = DOB.Value;
                int years = today.Year - birthDate.Year;
                int months = today.Month - birthDate.Month;
                int days = today.Day - birthDate.Day;

                if (days < 0)
                {
                    months--;
                    days += DateTime.DaysInMonth(today.Year, today.Month - 1);
                }

                if (months < 0)
                {
                    years--;
                    months += 12;
                }

                return $"{years}";
            }
        }

        [Display(Name = "Phone Number")]
        public string PhoneFormatted => "(" + Phone?.Substring(0, 3) + ") "
            + Phone?.Substring(3, 3) + "-" + Phone?[6..];

        [Display(Name = "Membership Number")]
        [Required(ErrorMessage = "Membership number is required.")]
        [Range(10000, 99999, ErrorMessage = "Membership number must be between 10000 and 99999")]
        public int MembershipNumber { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(45, ErrorMessage = "First name is limited to 45 characters.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(45, ErrorMessage = "Middle name is limited to 45 characters.")]
        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(55, ErrorMessage = "Last name is limited to 55 characters.")]
        public string LastName { get; set; } = "";

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        /*check if a phone number is valid*/
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; } = "";

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required.")]
        /*check if an email address is valid*/
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; } = "";

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal code is required.")]
        /*check if a postal code is valid*/
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code format.")]
        [StringLength(7)]
        public string PostalCode { get; set; } = "";

        [Display(Name = "Health Condition")]
        [Required(ErrorMessage = "Health condition is required.")]
        [StringLength(255, ErrorMessage = "Health condition is limited to 255 characters.")]
        public string HealthCondition { get; set; } = "";

        [Display(Name = "Notes")]
        [StringLength(2000, ErrorMessage = "Notes are limited to 2000 characters.")]
        public string? Notes { get; set; }

        [Display(Name = "Membership Start Date")]
        [Required(ErrorMessage = "Membership start date is required.")]
        [DataType(DataType.Date)]
        public DateTime MembershipStartDate { get; set; } = DateTime.Today;

        [Display(Name = "Membership End Date")]
        [Required(ErrorMessage = "Membership end date is required.")]
        [DataType(DataType.Date)]
        public DateTime MembershipEndDate { get; set; } = DateTime.Today.AddYears(1);

        [Display(Name = "Membership Fee")]
        [Required(ErrorMessage = "Membership fee is required.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(9,2)")]
        public double MembershipFee { get; set; } = 100.00;

        [Display(Name = "Fee Paid")]
        public bool FeePaid { get; set; } = false;

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Please select a membership type.")]
        public int MembershipTypeID { get; set; }

        [Display(Name = "Membership Type")]
        public MembershipType? MembershipType { get; set; }

        [Display(Name = "Group Classes")]
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();

        public ICollection<Workout> Workouts { get; set; } = new HashSet<Workout>();

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[]? RowVersion { get; set; }//Added for concurrency

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB?.AddYears(16) > DateTime.Today)
            {
                yield return new ValidationResult("Clients must be at least 16 years old.", ["DOB"]);
            }
        }
    }
}

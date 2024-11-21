using System.ComponentModel.DataAnnotations;

namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class Instructor : IValidatableObject
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

        [Display(Name = "Tenure as Instructor")]
        public string? Tenure
        {
            get
            {
                DateTime today = DateTime.Today;

                if (HireDate == null)
                {
                    return null;
                }

                if (HireDate > today)
                {
                    int daysUntilStart = (HireDate.Value - today).Days;
                    return $"Starts in {daysUntilStart} day{(daysUntilStart == 1 ? "" : "s")}";
                }

                /*including leap years*/
                DateTime hireDate = HireDate.Value;
                int years = today.Year - hireDate.Year;
                int months = today.Month - hireDate.Month;
                int days = today.Day - hireDate.Day;

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

                string yearString = years > 0 ? $"{years} year{(years == 1 ? "" : "s")}" : string.Empty;
                string monthString = months > 0 ? $"{months} month{(months == 1 ? "" : "s")}" : string.Empty;
                string dayString = days > 0 ? $"{days} day{(days == 1 ? "" : "s")}" : string.Empty;

                var parts = new List<string>();
                if (!string.IsNullOrEmpty(yearString)) parts.Add(yearString);
                if (!string.IsNullOrEmpty(monthString)) parts.Add(monthString);
                if (!string.IsNullOrEmpty(dayString)) parts.Add(dayString);

                if (parts.Count == 0)
                {
                    return "Less than a day";
                }

                /*insert "and" before the last part*/
                if (parts.Count > 1)
                {
                    var lastPart = parts.Last();
                    parts.RemoveAt(parts.Count - 1);
                    return string.Join(", ", parts) + " and " + lastPart;
                }

                return string.Join(", ", parts);
            }
        }

        [Display(Name = "Phone Number")]
        public string PhoneFormatted => "(" + Phone?.Substring(0, 3) + ") "
            + Phone?.Substring(3, 3) + "-" + Phone?[6..];
        
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

        [Display(Name = "Hire Date")]
        [Required(ErrorMessage = "Hire date is required.")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

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

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = false;

        public ICollection<GroupClass> GroupClasses { get; set; } = new HashSet<GroupClass>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime today = DateTime.Today;
            DateTime gymOpened = new DateTime(2018, 1, 1);
            DateTime maxFutureDate = today.AddMonths(1);

            if (HireDate.GetValueOrDefault() < gymOpened)
            {
                yield return new ValidationResult("Hire Date cannot be before January 1st, 2018 because that is when the Gym opened.", ["HireDate"]);
            }

            if (HireDate.GetValueOrDefault() > maxFutureDate)
            {
                yield return new ValidationResult("Hire Date cannot be more than one month in the future from the current date.", ["HireDate"]);
            }
        }
    }
}

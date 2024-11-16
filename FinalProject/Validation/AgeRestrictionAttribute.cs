using System.ComponentModel.DataAnnotations;

namespace FinalProject.Validation
{
    public class AgeRestrictionAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public AgeRestrictionAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dob)
            {
                var dobDateTime = dob.ToDateTime(TimeOnly.MinValue);
                var today = DateTime.Today;
                var age = today.Year - dobDateTime.Year;

                if (dobDateTime > today.AddYears(-age))
                {
                    --age;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old.");
                }
            }
            return ValidationResult.Success;
        }
    }
}

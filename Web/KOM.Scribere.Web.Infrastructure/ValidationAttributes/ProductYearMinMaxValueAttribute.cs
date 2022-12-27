namespace KOM.Scribere.Web.Infrastructure.ValidationAttributes;

using System;
using System.ComponentModel.DataAnnotations;

public class ProductYearMinMaxValueAttribute : ValidationAttribute
{
    public ProductYearMinMaxValueAttribute(int minYear)
    {
        this.MinYear = minYear;
    }

    public int MinYear { get; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is int intValue)
        {
            if (intValue <= DateTime.UtcNow.Year && intValue >= this.MinYear)
            {
                return ValidationResult.Success;
            }
        }

        if (value is DateTime dtValue)
        {
            if (dtValue.Year <= DateTime.UtcNow.Year && dtValue.Year >= this.MinYear)
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult($"Year should be between {this.MinYear} and {DateTime.UtcNow.Year}.");
    }
}
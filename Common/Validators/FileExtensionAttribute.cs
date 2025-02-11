using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Common.Validators
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public FileExtensionAttribute (string extensions)
        {
            _extensions = extensions.Split(',');
            ErrorMessage = "File type is not allowed.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Please select a file.");
            }

            if (value is string fileName)
            {
                var extension = Path.GetExtension(fileName);
                //If file type is not in the array
                if (Array.IndexOf(_extensions, extension.ToLower()) < 0)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}

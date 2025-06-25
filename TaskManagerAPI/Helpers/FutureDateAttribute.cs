using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Helpers
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return true;
        }
    }
} 
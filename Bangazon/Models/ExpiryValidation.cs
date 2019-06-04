using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bangazon.Models
{
    public sealed class ExpiryValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var paymentType = (PaymentType)validationContext.ObjectInstance;
            var expiryDate = paymentType.ExpiryDate;
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");

            if (paymentType.ExpiryDate == null)
            {
                return new ValidationResult(GetErrorMessage());
            }


            var dateParts = expiryDate.Split('/'); //expiry date in from MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
            {
                return new ValidationResult(GetErrorMessage());  // ^ check date format is valid as "MM/yyyy"
            }

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            if (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(GetErrorMessage());
        }
        

            public string GetErrorMessage()
        {
            return $"Invalid Expiration Date.";
        }
    }
}

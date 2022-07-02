using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpAuthenticationAndAuthorization.Validations
{
    public class ValidateEmailDomainAttribute:ValidationAttribute
    {
        private readonly string validDomains;

        public ValidateEmailDomainAttribute(string validDomains)
        {
            this.validDomains = validDomains;
        }
        public override bool IsValid(object value)
        {
            var splitEmail = value.ToString().Split('@');
            return (splitEmail[1].ToString().ToUpper() == this.validDomains.ToUpper());
         
        }

    }
}

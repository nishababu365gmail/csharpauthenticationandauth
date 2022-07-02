using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpAuthenticationAndAuthorization.Models.ViewModels
{
    public class IdentityRoleViewModel
    {
        [Required]

        public string Name { get; set; }

        public string Id { get; set; }
       
    }
}

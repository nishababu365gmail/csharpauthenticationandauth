using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpAuthenticationAndAuthorization.Models.ViewModels
{
    public class EditViewRoleModel
    {
        public EditViewRoleModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage="This Field is Required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}

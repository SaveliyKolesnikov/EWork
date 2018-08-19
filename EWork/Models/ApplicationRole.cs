using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EWork.Models
{
    public class ApplicationRole : IdentityRole
    {

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(string roleName, string description,
            DateTime createdDate)
            : base(roleName)
        {
            base.Name = roleName;

            this.Description = description;
            this.CreatedDate = createdDate;
        }

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}

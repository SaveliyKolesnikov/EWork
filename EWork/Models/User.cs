using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace EWork.Models
{
    public abstract class User : IdentityUser
    {
        [Required]
        [StringLength(24, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(24, MinimumLength = 2)]
        public string Surname { get; set; }

        public Balance Balance { get; set; }
        public List<Reference> References { get; set; }
        public List<Job> Jobs { get; set; }

        [NotMapped]
        public string FullName => $"{Name} {Surname}";
    }
}

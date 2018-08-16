using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EWork.Models
{
    public abstract class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [Range(0d, 10d, ErrorMessage = "{0} must be in the range 0..10")]
        [Display(Name="Rate")]
        public double Rate { get; set; }
        [Required]
        public Balance Balance { get; set; }
        public List<Job> Jobs { get; set; }

    }
}

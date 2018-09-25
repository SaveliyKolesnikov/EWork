using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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

        [StringLength(4096, ErrorMessage = "{0} length must be less then 4096")]
        [Display(Name = "Overview")]
        public string Description { get; set; }

        public bool IsBlocked { get; set; } = false;

        public string ProfilePhotoName { get; set; }

        [Required] public DateTime SignUpDate { get; set; }
        [Required] public Balance Balance { get; set; }
        
        public List<Review> Reviews { get; set; }
        public List<Review> SentReviews { get; set; }
        public List<Job> Jobs { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Message> SentMessages { get; set; }
        public List<Message> ReceivedMessages { get; set; }

        [NotMapped] public string FullName => $"{Name} {Surname}";

        [NotMapped] public abstract string Role { get; }
    }
}
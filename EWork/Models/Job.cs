using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public Employer Employer { get; set; }
        public Freelancer HiredFreelancer { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} length must be in the range 6..50")]
        [Display(Name="Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 30, ErrorMessage = "{0} length must be in the range 30..4096")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Budget { get; set; }

        public bool IsPaymentDenied { get; set; } = false;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }
        [Required]
        public List<Proposal> Proposals { get; set; }
        public List<JobTags> JobTags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models.Json
{
    public class JsonJob
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} length must be in the range 6..50")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 30, ErrorMessage = "{0} length must be in the range 30..4096")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Budget { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public double EmployerRating { get; set; }

        public string EmployerUserName { get; set; }
    }
}

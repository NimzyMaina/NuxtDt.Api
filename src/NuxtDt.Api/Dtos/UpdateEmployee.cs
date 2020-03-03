using System.ComponentModel.DataAnnotations;

namespace NuxtDt.Api.Dtos
{
    public class UpdateEmployee
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Office { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string Salary { get; set; }
    }
}
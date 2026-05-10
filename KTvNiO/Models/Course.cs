using System.ComponentModel.DataAnnotations;

namespace KTvNiO.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; }
        
        public string Content { get; set; } // Учебный материал
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsPublished { get; set; } = true;
    }
}

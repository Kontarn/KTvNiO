using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTvNiO.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }
        
        [Required]
        [StringLength(200)]
        public string OptionA { get; set; }
        
        [Required]
        [StringLength(200)]
        public string OptionB { get; set; }
        
        [StringLength(200)]
        public string OptionC { get; set; }
        
        [StringLength(200)]
        public string OptionD { get; set; }
        
        [Required]
        public char CorrectAnswer { get; set; } // A, B, C, or D
    }
}

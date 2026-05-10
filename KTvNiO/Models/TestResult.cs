using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTvNiO.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Score => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
        
        public DateTime TakenAt { get; set; } = DateTime.Now;
        public TimeSpan Duration { get; set; }
    }
}

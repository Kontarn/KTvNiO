using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace KTvNiO.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        
        public DateTime EnrolledAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}

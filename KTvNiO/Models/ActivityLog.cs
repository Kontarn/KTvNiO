using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace KTvNiO.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [Required]
        public string Action { get; set; } // Login, Logout, StartCourse, CompleteCourse, TakeTest, etc.
        
        public string Details { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        public TimeSpan? Duration { get; set; } // Для сессий
    }
}

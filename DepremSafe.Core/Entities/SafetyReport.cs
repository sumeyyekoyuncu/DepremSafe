using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Core.Entities
{
    [Table("SafetyReports")]
    public class SafetyReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Kullanıcı Id (Identity / Firebase / Custom User)
        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = null!;

        // Kullanıcı güvende mi?
        [Required]
        public bool IsSafe { get; set; }

        // Konum bilgileri (opsiyonel – GPS yoksa null olabilir)
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // GPS doğruluk değeri (metre)
        public float? Accuracy { get; set; }

        // Bildirimin gönderildiği zaman
        [Required]
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        // Kullanıcının kayıtlı şehri veya o an bulunduğu şehir
        [MaxLength(50)]
        public string? City { get; set; }

        // Kullanıcının ek notu (enkaz altındayım vb.)
        [MaxLength(500)]
        public string? Notes { get; set; }

        // 🔹 Opsiyonel: User entity ile ilişki
        // public AppUser User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
    [Table("VendorApplication")]
    public class VendorApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(255)]
        public string ContactInfo { get; set; } = default!;

        [Required]
        public VendorCategory Category { get; set; }

        // Reuse VendorStatus for application lifecycle (Pending/Approved/Rejected)
        public VendorStatus Status { get; set; } = VendorStatus.Pending;

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<VendorApplicationDocument>? Documents { get; set; }
    }
}
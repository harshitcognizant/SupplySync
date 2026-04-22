 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
    [Table("VendorApplicationDocument")]
    public class VendorApplicationDocument
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public int ApplicationID { get; set; }

        [Required]
        public VendorDocumentDocType DocType { get; set; }

        [Required]
        public string FileURI { get; set; } = default!;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        public VendorDocumentVerificationStatus VerificationStatus { get; set; } = VendorDocumentVerificationStatus.Pending;

        public bool IsDeleted { get; set; } = false;

        public virtual VendorApplication? Application { get; set; }
    }
}
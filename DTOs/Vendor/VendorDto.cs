using SupplySync.Constants.Enums;

namespace SupplySync.DTOs.Vendor
{
    public class CreateVendorApplicationDocumentDto
    {
        public VendorDocumentDocType DocType { get; set; }
        public string FileURI { get; set; } = default!;
    }

    public class CreateVendorApplicationRequestDto
    {
        public int UserID { get; set; }
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public List<CreateVendorApplicationDocumentDto>? Documents { get; set; }
    }

    public class CreateVendorDocumentRequestDto
    {
        public int? VendorID { get; set; }
        public VendorDocumentDocType DocType { get; set; }
        public IFormFile? DocFile { get; set; }
    }

    public class GetVendorFiltersRequestDto
    {
        public string? Name { get; set; }
        public VendorCategory? Category { get; set; }
        public VendorStatus? Status { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class UpdateVendorDocumentRequestDto
    {
        public VendorDocumentDocType? DocType { get; set; }
        public IFormFile? DocFile { get; set; }
    }

    public class UpdateVendorRequestDto
    {
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
    }

    public class VendorApplicationResponseDto
    {
        public int ApplicationID { get; set; }
        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VendorDocumentResponseDto>? Documents { get; set; }
    }

    public class VendorDocumentResponseDto
    {
        public int DocumentID { get; set; }
        public VendorDocumentDocType DocType { get; set; }
        public string FileURI { get; set; } = default!;
        public DateTime UploadedDate { get; set; }
        public VendorDocumentVerificationStatus VerificationStatus { get; set; }
    }

    public class VendorResponseDto
    {
        public int? UserId { get; set; }

        public string Name { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public VendorCategory Category { get; set; }
        public VendorStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }



}

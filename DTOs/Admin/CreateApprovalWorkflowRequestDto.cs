using SupplySync.Constants.Enums;
namespace SupplySync.DTOs.Admin {
    public class CreateApprovalWorkflowRequestDto { 
        public string Resource { get; set; } = default!;
        public RoleType ApproverRole {  get; set; }
        // e.g. "VendorApplication" public RoleType ApproverRole { get; set; }
    }
}
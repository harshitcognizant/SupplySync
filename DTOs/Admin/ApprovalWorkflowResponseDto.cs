using SupplySync.Constants.Enums;
namespace SupplySync.DTOs.Admin { 
    public class ApprovalWorkflowResponseDto { 
        public int WorkflowID { get; set; }
        public string Resource { get; set; } = default!;
        public RoleType ApproverRole { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
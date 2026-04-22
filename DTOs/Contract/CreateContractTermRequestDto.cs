using System;


namespace SupplySync.DTOs.Contract
{  

    public class CreateContractTermRequestDto
    {
        public int ContractID { get; set; }
        public string Description { get; set; } = default!;
        public bool ComplianceFlag { get; set; }
        public int? DeliveryTimelineDays { get; set; }
        public string? QualityRequirement { get; set; }
        public string? PenaltyClause { get; set; }
    }
}
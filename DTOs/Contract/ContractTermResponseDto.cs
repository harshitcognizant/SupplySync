using System;
using System.Collections.Generic;

namespace SupplySync.DTOs.Contract
{
    public class ContractTermResponseDto
    {
        public int TermID { get; set; }
        public int ContractID { get; set; }
        public string Description { get; set; } = default!;
        public bool ComplianceFlag { get; set; }
        public int? DeliveryTimelineDays { get; set; }
        public string? QualityRequirement { get; set; }
        public string? PenaltyClause { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContractWithTermsResponseDto : ContractResponseDto
    {
        public List<ContractTermResponseDto>? Terms { get; set; }
    }
}
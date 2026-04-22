using System;
using System.Collections.Generic;

namespace SupplySync.DTOs.Contract
{
    public class ContractResponseDto
    {
        public int ContractID { get; set; }
        public int VendorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Value { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    

     
}
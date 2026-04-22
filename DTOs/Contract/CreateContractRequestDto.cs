using System;
using System.Collections.Generic;

namespace SupplySync.DTOs.Contract
{
    public class CreateContractRequestDto
    {
        public int VendorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Value { get; set; }
        public List<CreateContractTermRequestDto>? Terms { get; set; }
    }

    
}
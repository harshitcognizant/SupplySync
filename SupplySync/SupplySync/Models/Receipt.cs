using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("Receipt")]
	public class Receipt
	{

			[Key]
			public int ReceiptID { get; set; }

			[Required]
			public int DeliveryID { get; set; } 

			[Required]
			public int WarehouseID { get; set; } 

			[Required]
			public DateOnly Date { get; set; }

			[Required] 
			public int Quantity { get; set; }

			[Required]
			public ReceiptStatus Status { get; set; }

			[Required]
			public bool IsDeleted { get; set; }

			public DateTime CreatedAt { get; set; } 
			public DateTime? UpdatedAt { get; set; }
			public virtual Warehouse Warehouse { get; set; } 
			public virtual Delivery Delivery { get; set; } 
		
	}

}

	
	


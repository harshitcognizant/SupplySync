using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupplySync.Constants.Enums;

namespace SupplySync.Models
{
	[Table("Inventory")]
	public class Inventory
	{
			[Key]
			public int InventoryID { get; set; }

			[Required]
			public int WarehouseID { get; set; } 

			[Required]
 			public string Item { get; set; } 

			[Required]
 			public int Quantity { get; set; }

			[Required]
			public DateOnly DateAdded { get; set; }

			[Required]
			public bool IsDeleted { get; set; }

		[Required]
 			public InventoryStatus Status { get; set; }
			public DateTime CreatedAt { get; set; }  
			public DateTime? UpdatedAt { get; set; }
			public virtual Warehouse Warehouse { get; set; }  
		
	}

}



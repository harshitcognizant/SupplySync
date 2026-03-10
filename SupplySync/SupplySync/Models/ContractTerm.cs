using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplySync.Models
{
	[Table("ContractTerm")]
	public class ContractTerm
	{
		[Key]
		public int TermID { get; set; }

		[Required]
		public int ContractID { get; set; }

		[ForeignKey("ContractID")]
		public virtual Contract Contract { get; set; } = null!;

		[Required]
		[Column(TypeName = "nvarchar(max)")]
		public string Description { get; set; } = string.Empty;

		[Required]
		public bool ComplianceFlag {  get; set; } = false;

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}

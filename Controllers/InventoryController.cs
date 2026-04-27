using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.InventoryandWarehouse;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryService _service;

		public InventoryController(IInventoryService service)
		{
			_service = service;
		}

		// CREATE
		[Authorize(Roles = "WarehouseManager")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateInventoryRequestDto dto)
		{
			var id = await _service.CreateAsync(dto);
			return Ok(new { Message = "Inventory created", InventoryID = id });
		}



        [Authorize(Roles = "WarehouseManager")]
        [HttpPost("issue")]
        public async Task<IActionResult> IssueStock([FromBody] IssueInventoryRequestDto dto)
        {
            await _service.IssueStockAsync(dto);
            return Ok(new { Message = "Stock issued successfully" });
        }



        // UPDATE
        [Authorize(Roles = "WarehouseManager")]
		[HttpPut("{inventoryId}")]
		public async Task<IActionResult> Update(int inventoryId, [FromBody] UpdateInventoryRequestDto dto)
		{
			var updated = await _service.UpdateAsync(inventoryId, dto);
			if (updated == null)
				return NotFound(new { Message = "Inventory not found" });

			return Ok(updated);
		}

		// GET BY ID
		[Authorize(Roles = "WarehouseManager")]
		[HttpGet("{inventoryId}")]
		public async Task<IActionResult> Get(int inventoryId)
		{
			var record = await _service.GetByIdAsync(inventoryId);
			if (record == null)
				return NotFound(new { Message = "Inventory not found" });

			return Ok(record);
		}

		// LIST
		[Authorize(Roles = "WarehouseManager")]
		[HttpGet]
		public async Task<IActionResult> List(
			[FromQuery] int? warehouseId,
			[FromQuery] string? item,
			[FromQuery] string? status,
			[FromQuery] DateOnly? fromDate,
			[FromQuery] DateOnly? toDate)
		{
			var list = await _service.ListAsync(warehouseId, item, status, fromDate, toDate);
			return Ok(list);
		}

		// DELETE
		[Authorize(Roles = "WarehouseManager")]
		[HttpDelete("{inventoryId}")]
		public async Task<IActionResult> Delete(int inventoryId)
		{
			var ok = await _service.DeleteAsync(inventoryId);
			if (!ok) return NotFound(new { Message = "Inventory not found" });

			return Ok(new { Message = "Inventory deleted" });
		}
	}
}
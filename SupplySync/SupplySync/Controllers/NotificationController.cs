using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Notification;
using SupplySync.Security;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // Route: /api/notification
	[Authorize]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		// ==========================================
		// USER ENDPOINTS (Self-Service)
		// ==========================================

		/// <summary>
		/// Retrieves notifications for a specific user.
		/// </summary>
		[HttpGet("my/{userId:int}")]
		public async Task<IActionResult> MyList(int userId, [FromQuery] NotificationFiltersDto filters)
		{
			var caller = User.GetUserId();
			if (!caller.HasValue) return Unauthorized(new { Message = "Invalid token." });

			try
			{
				var result = await _notificationService.ListMyAsync(userId, caller.Value, filters);
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
		}

		/// <summary>
		/// Updates a specific notification (e.g., mark as read, archive).
		/// </summary>
		[HttpPut("my/read/{id:int}")]
		public async Task<IActionResult> UpdateMy(int id, [FromBody] UpdateNotificationRequestDto dto)
		{
			var caller = User.GetUserId();
			if (!caller.HasValue) return Unauthorized(new { Message = "Invalid token." });

			try
			{
				var result = await _notificationService.UpdateMyAsync(id, caller.Value, dto, isAdmin: User.IsInRole("Admin"));
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
			catch (KeyNotFoundException) { return NotFound(new { Message = "Notification not found." }); }
			catch (InvalidOperationException ex) { return Conflict(new { Message = ex.Message }); }
		}

		/// <summary>
		/// Marks all notifications as read for a specific user.
		/// </summary>
		[HttpPut("my/{userId:int}/mark-all-read")]
		public async Task<IActionResult> MarkAllRead(int userId)
		{
			var caller = User.GetUserId();
			if (!caller.HasValue) return Unauthorized(new { Message = "Invalid token." });

			try
			{
				var count = await _notificationService.MarkAllAsReadAsync(userId, caller.Value, isAdmin: User.IsInRole("Admin"));
				return Ok(new { Updated = count });
			}
			catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
		}

		// ==========================================
		// ADMIN / INTERNAL ENDPOINTS
		// ==========================================

		/// <summary>
		/// Admin only: Get details of any specific notification.
		/// </summary>
		//[Authorize(Roles = "Admin")]
		[HttpGet("{id:int}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var result = await _notificationService.GetAdminAsync(id);
				return Ok(result);
			}
			catch (KeyNotFoundException) { return NotFound(new { Message = "Notification not found." }); }
		}

		/// <summary>
		/// Admin only: List all notifications across the system with filters.
		/// </summary>
		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> List([FromQuery] NotificationFiltersDto filters)
		{
			var result = await _notificationService.ListAdminAsync(filters);
			return Ok(result);
		}

		/// <summary>
		/// Admin only: Send bulk notifications to users or roles.
		/// </summary>
		[Authorize(Roles = "Admin")]
		[HttpPost("send")]
		public async Task<IActionResult> Send([FromBody] CreateBulkNotificationRequestDto dto)
		{
			try
			{
				var result = await _notificationService.SendAsync(dto);
				return Ok(result);
			}
			catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
		}
	}
}
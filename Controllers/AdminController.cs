using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Admin;
using SupplySync.DTOs.UserRoles;
using SupplySync.Services.Interfaces;

namespace SupplySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IVendorCategoryService _vendorCategoryService;
        private readonly IApprovalWorkflowService _workflowService;

        public AdminController(
            IUserService userService,
            IUserRoleService userRoleService,
            IVendorCategoryService vendorCategoryService,
            IApprovalWorkflowService workflowService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _vendorCategoryService = vendorCategoryService;
            _workflowService = workflowService;
        }

        // 1) Create user (uses existing CreateUserRequestDto and IUserService)
        [HttpPost("users/create")]
        public async Task<IActionResult> CreateUser([FromBody] DTOs.User.CreateUserRequestDto dto)
        {
            var id = await _userService.CreateUserAsync(dto);
            return Ok(new { Message = "User created", UserID = id });
        }

        // 2) Assign role to user
        [HttpPost("users/{userId}/assign-role")]
        public async Task<IActionResult> AssignRole([FromRoute] int userId, [FromBody] CreateUserRoleRequestDto dto)
        {
            var response = await _userRoleService.AssignRoleToUserAsync(userId, dto);
            return Ok(response);
        }

        // 3) Vendor categories (CRUD)
        [HttpPost("vendor-categories")]
        public async Task<IActionResult> CreateVendorCategory([FromBody] CreateVendorCategoryRequestDto dto)
        {
            var created = await _vendorCategoryService.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("vendor-categories")]
        public async Task<IActionResult> ListVendorCategories()
        {
            var list = await _vendorCategoryService.ListAsync();
            return Ok(list);
        }

        [HttpPut("vendor-categories/{id}")]
        public async Task<IActionResult> UpdateVendorCategory([FromRoute] int id, [FromBody] CreateVendorCategoryRequestDto dto)
        {
            var updated = await _vendorCategoryService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("vendor-categories/{id}")]
        public async Task<IActionResult> DeleteVendorCategory([FromRoute] int id)
        {
            await _vendorCategoryService.DeleteAsync(id);
            return Ok();
        }

        // 4) Approval workflows
        [HttpPost("approval-workflows")]
        public async Task<IActionResult> CreateApprovalWorkflow([FromBody] CreateApprovalWorkflowRequestDto dto)
        {
            var created = await _workflowService.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("approval-workflows")]
        public async Task<IActionResult> ListApprovalWorkflows()
        {
            var list = await _workflowService.ListAsync();
            return Ok(list);
        }

        [HttpDelete("approval-workflows/{id}")]
        public async Task<IActionResult> DeleteApprovalWorkflow([FromRoute] int id)
        {
            await _workflowService.DeleteAsync(id);
            return Ok();
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config; 
using SupplySync.DTOs.PurchaseOrder;
using SupplySync.Models;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseOrderController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponseDto>> CreatePurchaseOrder(CreatePurchaseOrderRequestDto request)
        {
            var purchaseOrder = _mapper.Map<PurchaseOrder>(request);

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<PurchaseOrderResponseDto>(purchaseOrder);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { poId = response.POID }, response);
        }

        [HttpPut("{poId}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> UpdatePurchaseOrder(int poId, UpdatePurchaseOrderRequestDto request)
        {
            var po = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.POID == poId && !x.IsDeleted);
            if (po == null) return NotFound();

            _mapper.Map(request, po);
            po.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<PurchaseOrderResponseDto>(po));
        }

        [HttpGet("{poId}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> GetPurchaseOrder(int poId)
        {
            var po = await _context.PurchaseOrders
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.POID == poId && !x.IsDeleted);

            if (po == null) return NotFound();

            return Ok(_mapper.Map<PurchaseOrderResponseDto>(po));
        }

        [HttpGet]
        public async Task<ActionResult<PurchaseOrderListResponseDto>> ListPurchaseOrders([FromQuery] string? itemFilter)
        {
            var query = _context.PurchaseOrders.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(itemFilter))
                query = query.Where(x => x.Item.Contains(itemFilter));

            var pos = await query.ToListAsync();

            return Ok(new PurchaseOrderListResponseDto
            {
                PurchaseOrders = _mapper.Map<List<PurchaseOrderResponseDto>>(pos),
                TotalCount = pos.Count
            });
        }
    }
}
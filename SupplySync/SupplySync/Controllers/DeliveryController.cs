using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.DTOs.Delivery;
using SupplySync.Models;

namespace SupplySync.Controllers
{
    [ApiController]
    [Route("api/deliveries")]
    public class DeliveryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DeliveryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryResponseDto>> CreateDelivery(CreateDeliveryRequestDto request)
        {
            // Verify PO exists before creating delivery
            var poExists = await _context.PurchaseOrders.AnyAsync(x => x.POID == request.POID);
            if (!poExists) return BadRequest("Invalid Purchase Order ID.");

            var delivery = _mapper.Map<Delivery>(request);

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<DeliveryResponseDto>(delivery));
        }

        [HttpPut("{deliveryId}")]
        public async Task<ActionResult<DeliveryResponseDto>> UpdateDelivery(int deliveryId, UpdateDeliveryRequestDto request)
        {
            var delivery = await _context.Deliveries.FirstOrDefaultAsync(x => x.DeliveryID == deliveryId && !x.IsDeleted);
            if (delivery == null) return NotFound();

            _mapper.Map(request, delivery);
            delivery.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<DeliveryResponseDto>(delivery));
        }

        [HttpGet("{deliveryId}")]
        public async Task<ActionResult<DeliveryResponseDto>> GetDelivery(int deliveryId)
        {
            var delivery = await _context.Deliveries
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.DeliveryID == deliveryId && !x.IsDeleted);

            if (delivery == null) return NotFound();

            return Ok(_mapper.Map<DeliveryResponseDto>(delivery));
        }

        [HttpGet]
        public async Task<ActionResult<DeliveryListResponseDto>> ListDeliveries([FromQuery] int? poId)
        {
            var query = _context.Deliveries.Where(x => !x.IsDeleted);

            if (poId.HasValue)
                query = query.Where(x => x.POID == poId.Value);

            var deliveries = await query.ToListAsync();

            return Ok(new DeliveryListResponseDto
            {
                Deliveries = _mapper.Map<List<DeliveryResponseDto>>(deliveries),
                TotalCount = deliveries.Count
            });
        }
    }
}
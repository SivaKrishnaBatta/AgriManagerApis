using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AgriManager.API.Data;
using AgriManager.API.Models;
using AgriManager.API.DTOs;

namespace AgriManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/crop-status")]
    public class CropStatusController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public CropStatusController(AgriManagerDbContext context)
        {
            _context = context;
        }

        // 🔑 Get CustomerId from JWT
        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ✅ CREATE CROP STATUS
        [HttpPost]
        public async Task<IActionResult> CreateCropStatus([FromBody] CropStatusCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var status = new CropStatus
            {
                CropStatusName = dto.CropStatusName,
                IsActive = dto.IsActive,
                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.CropStatuses.Add(status);
            await _context.SaveChangesAsync();

            return Ok(status);
        }

        // ✅ GET ALL CROP STATUS (Customer-wise)
        [HttpGet]
        public async Task<IActionResult> GetAllCropStatuses()
        {
            int customerId = GetCustomerId();

            var statuses = await _context.CropStatuses
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();

            return Ok(statuses);
        }

        // ✅ DROPDOWN (ONLY ID + NAME)
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCropStatusDropdown()
        {
            int customerId = GetCustomerId();

            var statuses = await _context.CropStatuses
                .Where(s => s.CustomerId == customerId && s.IsActive)
                .Select(s => new CropStatusDropdownDto
                {
                    CropStatusId = s.CropStatusId,
                    CropStatusName = s.CropStatusName
                })
                .ToListAsync();

            return Ok(statuses);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCropStatusById(int id)
        {
            int customerId = GetCustomerId();

            var status = await _context.CropStatuses
                .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

            if (status == null)
                return NotFound("Crop status not found");

            return Ok(status);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCropStatus(int id, [FromBody] CropStatusCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var status = await _context.CropStatuses
                .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

            if (status == null)
                return NotFound("Crop status not found");

            status.CropStatusName = dto.CropStatusName;
            status.IsActive = dto.IsActive;
            status.ModifiedAt = DateTime.Now;
            status.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(status);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCropStatus(int id)
        {
            int customerId = GetCustomerId();

            var status = await _context.CropStatuses
                .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

            if (status == null)
                return NotFound("Crop status not found");

            _context.CropStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return Ok("Crop status deleted successfully");
        }
    }
}

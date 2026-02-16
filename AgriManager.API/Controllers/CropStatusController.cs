using AgriManager.API.Data;
using AgriManager.API.DTOs;
using AgriManager.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllCropStatuses()
        {
            var statuses = await _context.CropStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.CropStatusId)
                .ToListAsync();

            return Ok(new ApiResponseDto<List<CropStatus>>
            {
                Status = true,
                Message = "Crop statuses fetched successfully",
                Data = statuses
            });
        }

        // ================= DROPDOWN =================
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCropStatusDropdown()
        {
            var statuses = await _context.CropStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.CropStatusId)
                .Select(s => new CropStatusDropdownDto
                {
                    CropStatusId = s.CropStatusId,
                    CropStatusName = s.CropStatusName
                })
                .ToListAsync();

            return Ok(new ApiResponseDto<List<CropStatusDropdownDto>>
            {
                Status = true,
                Message = "Dropdown data fetched successfully",
                Data = statuses
            });
        }
    }
}

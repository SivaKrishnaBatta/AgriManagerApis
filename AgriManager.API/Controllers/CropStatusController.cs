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

        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateCropStatus([FromBody] CropStatusCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                if (string.IsNullOrWhiteSpace(dto.CropStatusName))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Crop status name is required",
                        Data = null
                    });
                }

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

                return Ok(new ApiResponseDto<CropStatus>
                {
                    Status = true,
                    Message = "Crop status created successfully",
                    Data = status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllCropStatuses()
        {
            try
            {
                int customerId = GetCustomerId();

                var statuses = await _context.CropStatuses
                    .Where(s => s.CustomerId == customerId)
                    .ToListAsync();

                return Ok(new ApiResponseDto<List<CropStatus>>
                {
                    Status = true,
                    Message = "Crop statuses fetched successfully",
                    Data = statuses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= DROPDOWN =================
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCropStatusDropdown()
        {
            try
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

                return Ok(new ApiResponseDto<List<CropStatusDropdownDto>>
                {
                    Status = true,
                    Message = "Dropdown data fetched successfully",
                    Data = statuses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCropStatusById(int id)
        {
            try
            {
                int customerId = GetCustomerId();

                var status = await _context.CropStatuses
                    .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

                if (status == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Crop status not found",
                        Data = null
                    });
                }

                return Ok(new ApiResponseDto<CropStatus>
                {
                    Status = true,
                    Message = "Crop status fetched successfully",
                    Data = status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCropStatus(int id, [FromBody] CropStatusCreateUpdateDto dto)
        {
            try
            {
                int customerId = GetCustomerId();

                var status = await _context.CropStatuses
                    .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

                if (status == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Crop status not found",
                        Data = null
                    });
                }

                status.CropStatusName = dto.CropStatusName;
                status.IsActive = dto.IsActive;
                status.ModifiedAt = DateTime.Now;
                status.ModifiedBy = customerId;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<CropStatus>
                {
                    Status = true,
                    Message = "Crop status updated successfully",
                    Data = status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCropStatus(int id)
        {
            try
            {
                int customerId = GetCustomerId();

                var status = await _context.CropStatuses
                    .FirstOrDefaultAsync(s => s.CropStatusId == id && s.CustomerId == customerId);

                if (status == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Status = false,
                        Message = "Crop status not found",
                        Data = null
                    });
                }

                _context.CropStatuses.Remove(status);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDto<object>
                {
                    Status = true,
                    Message = "Crop status deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}

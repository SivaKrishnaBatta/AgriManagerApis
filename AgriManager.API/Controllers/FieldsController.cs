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
    [Route("api/fields")]
    public class FieldsController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public FieldsController(AgriManagerDbContext context)
        {
            _context = context;
        }

        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ✅ CREATE FIELD
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<Field>>> CreateField(
            [FromBody] FieldCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var field = new Field
            {
                FarmId = dto.FarmId,
                FieldName = dto.FieldName,
                Area = dto.Area,
                Notes = dto.Notes,
                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<Field>
            {
                Status = true,
                Message = "Field created successfully",
                Data = field
            });
        }

        // ✅ GET ALL FIELDS
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<FieldResponseDto>>>> GetAllFields()
        {
            int customerId = GetCustomerId();

            var fields = await (
                from field in _context.Fields
                join farm in _context.Farms
                    on field.FarmId equals farm.FarmId
                where field.CustomerId == customerId
                select new FieldResponseDto
                {
                    FieldId = field.FieldId,
                    FarmId = field.FarmId,
                    FarmName = farm.FarmName,
                    FieldName = field.FieldName,
                    Area = field.Area,
                    Notes = field.Notes
                }
            ).ToListAsync();

            return Ok(new ApiResponseDto<List<FieldResponseDto>>
            {
                Status = true,
                Message = "Fields fetched successfully",
                Data = fields
            });
        }

        // ✅ GET FIELD BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<FieldResponseDto>>> GetFieldById(int id)
        {
            int customerId = GetCustomerId();

            var field = await (
                from f in _context.Fields
                join farm in _context.Farms
                    on f.FarmId equals farm.FarmId
                where f.FieldId == id && f.CustomerId == customerId
                select new FieldResponseDto
                {
                    FieldId = f.FieldId,
                    FarmId = f.FarmId,
                    FarmName = farm.FarmName,
                    FieldName = f.FieldName,
                    Area = f.Area,
                    Notes = f.Notes
                }
            ).FirstOrDefaultAsync();

            if (field == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Field not found"
                });
            }

            return Ok(new ApiResponseDto<FieldResponseDto>
            {
                Status = true,
                Message = "Field fetched successfully",
                Data = field
            });
        }

        // ✅ UPDATE FIELD
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<Field>>> UpdateField(
            int id, [FromBody] FieldCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            var field = await _context.Fields
                .FirstOrDefaultAsync(f => f.FieldId == id && f.CustomerId == customerId);

            if (field == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Field not found"
                });
            }

            field.FarmId = dto.FarmId;
            field.FieldName = dto.FieldName;
            field.Area = dto.Area;
            field.Notes = dto.Notes;
            field.ModifiedAt = DateTime.Now;
            field.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<Field>
            {
                Status = true,
                Message = "Field updated successfully",
                Data = field
            });
        }

        // ✅ DELETE FIELD
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteField(int id)
        {
            int customerId = GetCustomerId();

            var field = await _context.Fields
                .FirstOrDefaultAsync(f => f.FieldId == id && f.CustomerId == customerId);

            if (field == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Status = false,
                    Message = "Field not found"
                });
            }

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<object>
            {
                Status = true,
                Message = "Field deleted successfully"
            });
        }
    }
}

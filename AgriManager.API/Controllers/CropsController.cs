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
    [Route("api/crops")]
    public class CropsController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;

        public CropsController(AgriManagerDbContext context)
        {
            _context = context;
        }

        // 🔑 Get CustomerId from JWT
        private int GetCustomerId()
        {
            return int.Parse(User.FindFirst("CustomerId")!.Value);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> CreateCrop([FromBody] CropCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            // 🔒 Validation: EndDate >= StartDate
            if (dto.ExpectedEndDate.HasValue &&
                dto.ExpectedEndDate.Value.Date < dto.StartDate.Date)
            {
                return BadRequest("End Date cannot be earlier than Start Date");
            }

            var crop = new Crop
            {
                FarmId = dto.FarmId,
                FieldId = dto.FieldId,
                CropName = dto.CropName,
                CropStatusId = dto.CropStatusId,
                Season = dto.Season,

                StartDate = DateOnly.FromDateTime(dto.StartDate),
                ExpectedEndDate = dto.ExpectedEndDate.HasValue
                    ? DateOnly.FromDateTime(dto.ExpectedEndDate.Value)
                    : null,

                ExpectedYield = dto.ExpectedYield,

                CustomerId = customerId,
                CreatedBy = customerId,
                CreatedAt = DateTime.Now
            };

            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();

            return Ok(crop);
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAllCrops()
        {
            int customerId = GetCustomerId();

            var crops = await (
                from crop in _context.Crops
                join farm in _context.Farms on crop.FarmId equals farm.FarmId
                join field in _context.Fields on crop.FieldId equals field.FieldId
                join status in _context.CropStatuses on crop.CropStatusId equals status.CropStatusId
                where crop.CustomerId == customerId
                select new CropListDto
                {
                    CropId = crop.CropId,

                    FarmId = farm.FarmId,
                    FarmName = farm.FarmName,

                    FieldId = field.FieldId,
                    FieldName = field.FieldName,

                    CropName = crop.CropName,

                    CropStatusId = status.CropStatusId,
                    CropStatusName = status.CropStatusName,

                    Season = crop.Season,
                    StartDate = crop.StartDate,
                    ExpectedEndDate = crop.ExpectedEndDate,
                    ExpectedYield = crop.ExpectedYield
                }
            ).ToListAsync();

            return Ok(crops);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCropById(int id)
        {
            int customerId = GetCustomerId();

            var crop = await (
                from c in _context.Crops
                join f in _context.Farms on c.FarmId equals f.FarmId
                join fd in _context.Fields on c.FieldId equals fd.FieldId
                join s in _context.CropStatuses on c.CropStatusId equals s.CropStatusId
                where c.CropId == id && c.CustomerId == customerId
                select new CropListDto
                {
                    CropId = c.CropId,
                    FarmId = f.FarmId,
                    FarmName = f.FarmName,
                    FieldId = fd.FieldId,
                    FieldName = fd.FieldName,
                    CropName = c.CropName,
                    CropStatusId = s.CropStatusId,
                    CropStatusName = s.CropStatusName,
                    Season = c.Season,
                    StartDate = c.StartDate,
                    ExpectedEndDate = c.ExpectedEndDate,
                    ExpectedYield = c.ExpectedYield
                }
            ).FirstOrDefaultAsync();

            if (crop == null)
                return NotFound("Crop not found");

            return Ok(crop);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCrop(int id, [FromBody] CropCreateUpdateDto dto)
        {
            int customerId = GetCustomerId();

            // 🔒 Validation: EndDate >= StartDate
            if (dto.ExpectedEndDate.HasValue &&
                dto.ExpectedEndDate.Value.Date < dto.StartDate.Date)
            {
                return BadRequest("End Date cannot be earlier than Start Date");
            }

            var crop = await _context.Crops
                .FirstOrDefaultAsync(c => c.CropId == id && c.CustomerId == customerId);

            if (crop == null)
                return NotFound("Crop not found");

            crop.FarmId = dto.FarmId;
            crop.FieldId = dto.FieldId;
            crop.CropName = dto.CropName;
            crop.CropStatusId = dto.CropStatusId;
            crop.Season = dto.Season;

            crop.StartDate = DateOnly.FromDateTime(dto.StartDate);
            crop.ExpectedEndDate = dto.ExpectedEndDate.HasValue
                ? DateOnly.FromDateTime(dto.ExpectedEndDate.Value)
                : null;

            crop.ExpectedYield = dto.ExpectedYield;

            crop.ModifiedAt = DateTime.Now;
            crop.ModifiedBy = customerId;

            await _context.SaveChangesAsync();

            return Ok(crop);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrop(int id)
        {
            int customerId = GetCustomerId();

            var crop = await _context.Crops
                .FirstOrDefaultAsync(c => c.CropId == id && c.CustomerId == customerId);

            if (crop == null)
                return NotFound("Crop not found");

            _context.Crops.Remove(crop);
            await _context.SaveChangesAsync();

            return Ok("Crop deleted successfully");
        }
    }
}

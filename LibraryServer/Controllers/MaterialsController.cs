using LibraryServer.DTO.Material;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly MaterialService _materialService;

        public MaterialsController(MaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(string? search = null)
        {
            try
            {
                var materials = await _materialService.GetAll(search);
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var material = await _materialService.GetById(id);
                return Ok(material);
            }
            catch (Exception ex)
            {
                return NotFound(new { msg = ex.Message });
            }
        }

        [HttpPost("upload")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        [Authorize(Roles = "Librarian, Teacher")]
        public async Task<IActionResult> UploadMaterial([FromForm] UploadMaterialDto uploadMaterialDto)
        {
            try
            {
                var path = await _materialService.UploadMaterial(uploadMaterialDto);
                return Ok(new { msg = "Материал загружен", path = path });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        [Authorize(Roles = "Librarian, Teacher")]
        public async Task<IActionResult> UpdateMaterial([FromRoute] int id, [FromForm] UpdateMaterialsDto dto)
        {
            try
            {
                var material = await _materialService.UpdateMaterial(id, dto.Name, dto.File);
                return Ok(new { msg = "Материал обновлен", data = material });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await _materialService.Delete(id);
                return Ok(new { msg = "Материал удален", access = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
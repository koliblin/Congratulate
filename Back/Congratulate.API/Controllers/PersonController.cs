using Congratulate.Application.Interfaces;
using Congratulate.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Congratulate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _service;
        private readonly IWebHostEnvironment _env;

        public PersonController(IPersonService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _service.GetAllAsync();
            return Ok(people);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
        {
            var people = await _service.GetUpcomingAsync(days);
            return Ok(people);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _service.GetByIdAsync(id);
            if (person == null) return NotFound();
            return Ok(person);
        }

        public class PersonCreateRequest
        {
            [Required(ErrorMessage = "Имя обязательно")]
            [StringLength(100, ErrorMessage = "Имя слишком длинное")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Дата рождения обязательна")]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }

            public IFormFile? Photo { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PersonCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = new Person { Name = request.Name, Birthday = DateTime.SpecifyKind(request.Birthday, DateTimeKind.Utc) };
            if (request.Photo != null)
            {
                using var stream = request.Photo.OpenReadStream();
                person = await _service.AddAsync(person, stream, request.Photo.FileName);
            }
            else
            {
                person = await _service.AddAsync(person);
            }
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
        }

        public class PersonUpdateRequest
        {
            [Required(ErrorMessage = "Имя обязательно")]
            [StringLength(100, ErrorMessage = "Имя слишком длинное")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Дата рождения обязательна")]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }

            public IFormFile? Photo { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PersonUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = await _service.GetByIdAsync(id);
            if (person == null) return NotFound();
            person.Name = request.Name;
            person.Birthday = DateTime.SpecifyKind(request.Birthday, DateTimeKind.Utc);
            if (request.Photo != null)
            {
                using var stream = request.Photo.OpenReadStream();
                person = (await _service.UpdateAsync(person, stream, request.Photo.FileName))!;
            }
            else
            {
                person = (await _service.UpdateAsync(person))!;
            }
            return Ok(person);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("photo/{fileName}")]
        public IActionResult GetPhoto(string fileName)
        {
            var photoPath = Path.Combine(_env.ContentRootPath, "Photos", fileName);
            if (!System.IO.File.Exists(photoPath)) return NotFound();
            var contentType = "image/jpeg";
            return PhysicalFile(photoPath, contentType);
        }
    }
} 
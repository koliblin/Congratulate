using Congratulate.Application.Interfaces;
using Congratulate.Domain.Models;
using Congratulate.Infrastructure.Interfaces;

namespace Congratulate.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly string _photoDirectory;

        public PersonService(IPersonRepository repository, string photoDirectory)
        {
            _repository = repository;
            _photoDirectory = photoDirectory;
        }

        public Task<IEnumerable<Person>> GetAllAsync() => _repository.GetAllAsync();
        public Task<IEnumerable<Person>> GetUpcomingAsync(int daysAhead = 7) => _repository.GetUpcomingAsync(daysAhead);
        public Task<Person?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public async Task<Person> AddAsync(Person person, Stream? photoStream = null, string? photoFileName = null)
        {
            if (photoStream != null && !string.IsNullOrEmpty(photoFileName))
            {
                var path = Path.Combine(_photoDirectory, photoFileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photoStream.CopyToAsync(fileStream);
                }
                person.PhotoPath = photoFileName;
            }
            return await _repository.AddAsync(person);
        }

        public async Task<Person?> UpdateAsync(Person person, Stream? photoStream = null, string? photoFileName = null)
        {
            if (photoStream != null && !string.IsNullOrEmpty(photoFileName))
            {
                var path = Path.Combine(_photoDirectory, photoFileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photoStream.CopyToAsync(fileStream);
                }
                person.PhotoPath = photoFileName;
            }
            return await _repository.UpdateAsync(person);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _repository.GetByIdAsync(id);
            if (person == null) return false;

            var result = await _repository.DeleteAsync(id);
            if (result && !string.IsNullOrEmpty(person.PhotoPath))
            {
                var photoPath = Path.Combine(_photoDirectory, person.PhotoPath);
                if (File.Exists(photoPath))
                    File.Delete(photoPath);
            }
            return result;
        }
    }
} 
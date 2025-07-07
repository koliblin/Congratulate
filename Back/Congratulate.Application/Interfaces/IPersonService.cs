using Congratulate.Domain.Models;

namespace Congratulate.Application.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<IEnumerable<Person>> GetUpcomingAsync(int daysAhead = 7);
        Task<Person?> GetByIdAsync(int id);
        Task<Person> AddAsync(Person person, Stream? photoStream = null, string? photoFileName = null);
        Task<Person?> UpdateAsync(Person person, Stream? photoStream = null, string? photoFileName = null);
        Task<bool> DeleteAsync(int id);
    }
} 
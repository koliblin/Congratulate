using Congratulate.Domain.Models;

namespace Congratulate.Infrastructure.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<IEnumerable<Person>> GetUpcomingAsync(int daysAhead = 7);
        Task<Person?> GetByIdAsync(int id);
        Task<Person> AddAsync(Person person);
        Task<Person?> UpdateAsync(Person person);
        Task<bool> DeleteAsync(int id);
    }
} 
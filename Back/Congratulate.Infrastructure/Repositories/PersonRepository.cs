using Congratulate.Domain.Models;
using Congratulate.Infrastructure.Data;
using Congratulate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Congratulate.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly BirthdayContext _context;
        public PersonRepository(BirthdayContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
            => await _context.Persons.ToListAsync();

        public async Task<IEnumerable<Person>> GetUpcomingAsync(int daysAhead = 7)
        {
            var today = DateTime.UtcNow.Date;
            var end = today.AddDays(daysAhead);
            int startDay = today.DayOfYear;
            int endDay = end.DayOfYear;

            var people = await _context.Persons.ToListAsync();

            return people.Where(p =>
            {
                var bday = DateTime.SpecifyKind(p.Birthday, DateTimeKind.Utc);
                var bdayThisYear = new DateTime(today.Year, bday.Month, bday.Day);
                int bdayDayOfYear = bdayThisYear.DayOfYear;
                if (endDay >= startDay)
                    return bdayDayOfYear >= startDay && bdayDayOfYear <= endDay;
                else 
                    return bdayDayOfYear >= startDay || bdayDayOfYear <= endDay;
            });
        }

        public async Task<Person?> GetByIdAsync(int id)
            => await _context.Persons.FindAsync(id);

        public async Task<Person> AddAsync(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person?> UpdateAsync(Person person)
        {
            var existing = await _context.Persons.FindAsync(person.Id);
            if (existing == null) return null;
            existing.Name = person.Name;
            existing.Birthday = person.Birthday;
            existing.PhotoPath = person.PhotoPath;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return false;
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 
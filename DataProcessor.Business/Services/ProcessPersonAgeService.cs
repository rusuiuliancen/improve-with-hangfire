using DataProcessor.DataAccess;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessor.Business.Services
{
    public class ProcessPersonAgeService
    {
        private readonly AppDbContext _dbContext;

        public ProcessPersonAgeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GenerateRecurringJobsForAllPersons(string tenantKey)
        {
            var persons = await _dbContext.Persons.ToListAsync();
            foreach (var person in persons)
            {
                var jobId = $"recalculate-age-{person.Id}@" + tenantKey;
                RecurringJob.AddOrUpdate(jobId, "persons-queue", () => RecalculateAge(person.Id), Cron.Minutely);
            }
        }

        public void RecalculateAge(Guid personId)
        {
            Task.Delay(1000).Wait();
            var person = _dbContext.Persons.FirstOrDefault(p => p.Id == personId);
            if (person != null)
            {
                var today = DateTime.Today;
                var dob = person.DateOfBirth;
                person.Age = today.Year - dob.Year - (dob > today.AddYears(-(today.Year - dob.Year)) ? 1 : 0);
                _dbContext.SaveChanges();
            }
        }
    }
}

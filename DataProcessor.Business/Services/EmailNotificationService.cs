using DataProcessor.Business.Contracts;
using DataProcessor.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessor.Business.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(AppDbContext dbContext, ILogger<EmailNotificationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public void SendNotifications()
        {
            var random = new Random();
            try
            {
                Task.Delay(200).Wait();

                var persons = _dbContext.Persons.Where(p=>!p.EmailSent);
                foreach (var person in persons)
                {
                    if (random.Next(10) == 0)
                    {
                        person.ProcessException = "[Email] Something went wrong.";
                        _dbContext.SaveChanges();
                        continue;
                    }
                    else
                    {
                        person.EmailSent = true;
                        person.ProcessException = null;
                        _dbContext.SaveChanges();
                    }

                }               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email notifications.");
                throw;
            }
        }
    }
}

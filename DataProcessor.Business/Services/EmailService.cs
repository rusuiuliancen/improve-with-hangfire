using DataProcessor.Business.Contracts;
using DataProcessor.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessor.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EmailService> _logger;

        public EmailService(AppDbContext dbContext, ILogger<EmailService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public void SendEmail(string to, string subject, string body)
        {
            var random = new Random();
            try
            {
                Task.Delay(200).Wait();

                var person = _dbContext.Persons.FirstOrDefault(p => p.Email == to);
                if (person != null)
                {
                    if (random.Next(10) == 0)
                    {
                        person.ProcessException = "[Email] Something went wrong.";
                        _dbContext.SaveChanges();
                        throw new Exception("Simulated email sending failure.");
                    }
                    else
                    {
                        person.EmailSent = true;
                        person.ProcessException = null;
                        _dbContext.SaveChanges();
                    }

                }               

                _logger.LogInformation($"Email sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }
    }
}

using DataProcessor.Business.Services;
using Hangfire;

namespace DataProcessor.API.Hangfire
{
    public class JobRegistry
    {
        public void RegisterJobs(IServiceProvider serviceProvider)
        {
            var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
            //jobManager.AddOrUpdate<EmailService>("SendNotifications@tenant1", x => x.SendNotifications(), Cron.Daily);
            //jobManager.AddOrUpdate<EmailService>("SendNotifications@tenant2", x => x.SendNotifications(), Cron.Monthly);
            jobManager.AddOrUpdate<ProcessAllPersonService>("AllPersons@tenant1", x => x.GenerateRecurringJobsForAllPersons("tenant1"), Cron.Minutely);
            //jobManager.AddOrUpdate<ProcessAllPersonService>("AllPersons@tenant2", x => x.GenerateRecurringJobsForAllPersons("tenant2"), Cron.Minutely);
        }
    }
}

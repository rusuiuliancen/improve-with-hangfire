using Hangfire;

namespace DataProcessor.API.Hangfire
{
    public class JobRegistry
    {
        public void RegisterJobs(IServiceProvider serviceProvider)
        {
            var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
        }
    }
}

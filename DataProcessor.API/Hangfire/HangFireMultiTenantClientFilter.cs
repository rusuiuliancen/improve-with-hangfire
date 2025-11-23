using Hangfire.Client;

namespace DataProcessor.API.Hangfire
{
    public class HangFireMultiTenantClientFilter : IClientFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {
            filterContext.Parameters.TryGetValue("RecurringJobId", out _);
            filterContext.Parameters.TryGetValue("tenant", out _);
        }

        public void OnCreating(CreatingContext filterContext)
        {
            if (filterContext.Parameters.TryGetValue("RecurringJobId", out object jobId) && jobId != null && jobId.ToString().Contains('@'))
            {
                var hostname = jobId.ToString().Split('@').Last();
                filterContext.SetJobParameter("tenant", hostname);
            }
        }
    }
}

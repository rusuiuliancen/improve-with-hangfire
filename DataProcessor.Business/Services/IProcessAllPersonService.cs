using System.Threading.Tasks;

namespace DataProcessor.Business.Services
{
    public interface IProcessAllPersonService
    {
        Task GenerateRecurringJobsForAllPersons(string tenantKey);
    }
}

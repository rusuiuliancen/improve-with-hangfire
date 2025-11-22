using System.Threading.Tasks;

namespace DataProcessor.Business.Contracts
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}

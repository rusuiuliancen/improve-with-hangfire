using System.Threading.Tasks;

namespace DataProcessor.Business.Contracts
{
    public interface IEmailSender
    {
        void SendEmail(string to, string subject, string body);
    }
}

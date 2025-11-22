using DataProcessor.Business.Dtos;
using System.IO;

namespace DataProcessor.Business.Contracts
{
    public interface IPersonProcessorService
    {
        PersonProcessResult ProcessCsv(Stream csvStream);
    }
}

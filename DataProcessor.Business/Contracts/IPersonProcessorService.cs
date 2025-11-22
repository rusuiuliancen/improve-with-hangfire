using DataProcessor.Business.Dtos;
using System.IO;

namespace DataProcessor.Business.Contracts
{
    public interface IPersonProcessorService
    {
        CsvProcessResult ProcessCsv(Stream csvStream);
    }
}

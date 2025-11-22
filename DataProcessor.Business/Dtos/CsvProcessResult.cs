namespace DataProcessor.Business.Dtos
{
    public class CsvProcessResult
    {
        public int TotalProcessed { get; set; }
        public int TotalInserted { get; set; }
        public int TotalInvalid { get; set; }
    }
}

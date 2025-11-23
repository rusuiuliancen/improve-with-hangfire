namespace DataProcessor.Business.Tenant
{
    public class TenantModel
    {
        public TenantModel()
        {
            
        }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public TenantModel(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }
    }
}

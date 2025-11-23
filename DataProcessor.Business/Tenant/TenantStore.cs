using System.Collections.Generic;

namespace DataProcessor.Business.Tenant
{
    public static class TenantStore
    {
        public static readonly Dictionary<string, TenantModel> TenantDictionary = new()
        {
            { "tenant1",  new TenantModel("tenant1", "Server=(localdb)\\MSSQLLocalDB;Database=DataProcessorDbT1;Trusted_Connection=True;MultipleActiveResultSets=true")},
            { "tenant2",  new TenantModel("tenant2", "Server=(localdb)\\MSSQLLocalDB;Database=DataProcessorDbT2;Trusted_Connection=True;MultipleActiveResultSets=true")}
        };
    }
}

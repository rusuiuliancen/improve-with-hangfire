namespace DataProcessor.Business.Tenant
{
    public class TenantResolver : ITenantResolver
    {
        public TenantModel Tenant { get; set; }

        public void SetTenant(TenantModel tenant)
        {
            Tenant = tenant;
        }

        public TenantModel TryGetTenant()
        {
            return Tenant;
        }
    }
}


namespace DataProcessor.Business.Tenant
{
    public interface ITenantResolver
    {
        TenantModel TryGetTenant();
        void SetTenant(TenantModel tenant);
    }
}

using Hangfire;
using DataProcessor.DataAccess;
using Microsoft.EntityFrameworkCore;
using DataProcessor.Business.Tenant;

namespace DataProcessor.API.Hangfire
{
    public partial class TenantJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;
        public TenantJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            return _serviceProvider.GetRequiredService(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            var tenantKey = context.GetJobParameter<string>("tenant");
            TenantModel tenant = null;
            if (!string.IsNullOrEmpty(tenantKey))
            {
                TenantStore.TenantDictionary.TryGetValue(tenantKey, out tenant);
            }
            var scope = _serviceProvider.CreateScope();

            AppDbContext dbContext = null;
            if (tenant != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
                dbContext = new AppDbContext(optionsBuilder.Options);

                var tenantResolver = scope.ServiceProvider.GetRequiredService<ITenantResolver>();
                if (tenantResolver != null)
                {
                    tenantResolver.SetTenant(tenant);
                }
            }

            return new TenantJobActivatorScope(scope, dbContext);
        }
    }
}

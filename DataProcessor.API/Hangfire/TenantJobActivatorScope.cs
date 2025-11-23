using Hangfire;
using DataProcessor.DataAccess;

namespace DataProcessor.API.Hangfire
{
    public class TenantJobActivatorScope : JobActivatorScope
    {
        private readonly IServiceScope _scope;
        private readonly AppDbContext _dbContext;

        public TenantJobActivatorScope(IServiceScope scope, AppDbContext dbContext)
        {
            _scope = scope;
            _dbContext = dbContext;
        }

        public override object Resolve(Type type)
        {
            if (type == typeof(AppDbContext) && _dbContext != null)
                return _dbContext;
            return _scope.ServiceProvider.GetRequiredService(type);
        }

        public override void DisposeScope()
        {
            _dbContext?.Dispose();
            _scope.Dispose();
        }
    }
}

using DataProcessor.Business.Tenant;

namespace DataProcessor.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver)
        {
            var tenantKey = context.Request.Headers["x-request-tenant"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantKey))
            {
                tenantKey = context.Request.Host.Host;
            }
            TenantStore.TenantDictionary.TryGetValue(tenantKey, out var tenant);
            tenantResolver.SetTenant(tenant);

            await _next(context);
        }
    }
}

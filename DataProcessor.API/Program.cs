using DataProcessor.API.Hangfire;
using DataProcessor.API.Middlewares;
using DataProcessor.API.Swagger;
using DataProcessor.Business.Contracts;
using DataProcessor.Business.Services;
using DataProcessor.Business.Tenant;
using DataProcessor.DataAccess;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace DataProcessor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Hangfire services
            //builder.Services.AddHangfireServer();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerTenantHeaderFilter>();
            });

            builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            builder.Services.AddScoped<EmailNotificationService>();
            builder.Services.AddScoped<IPersonProcessorService, PersonProcessorService>();
            builder.Services.AddScoped<PersonProcessorService>();
            builder.Services.AddScoped<ITenantResolver, TenantResolver>();
            builder.Services.AddScoped<TenantModel>();

            builder.Services.AddScoped<AppDbContext>(provider =>
            {
                var tenantResolver = provider.GetRequiredService<ITenantResolver>();
                var tenant = tenantResolver.TryGetTenant();

                if (tenant == null)
                    tenant = TenantStore.TenantDictionary.Values.First();

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
                return new AppDbContext(optionsBuilder.Options);
            });
            
            //builder.Services.AddHangfire((provider, config) =>
            //{
            //    config.UseSimpleAssemblyNameTypeSerializer()
            //          .UseRecommendedSerializerSettings()
            //          .UseSqlServerStorage("Server=(localdb)\\MSSQLLocalDB;Database=DataProcessorDb_Hangfire;Trusted_Connection=True;MultipleActiveResultSets=true")
            //          .UseFilter(new HangFireMultiTenantClientFilter())
            //          .UseActivator(new TenantJobActivator(provider));
            //});

            builder.Services.AddHttpContextAccessor();

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            var app = builder.Build();

            //var jobRegistry = new JobRegistry();
            //jobRegistry.RegisterJobs(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMiddleware<TenantMiddleware>();

            //app.UseHangfireDashboard("/hangfire");
            app.MapControllers();
            app.Run();
        }
    }
}

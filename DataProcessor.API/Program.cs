using DataProcessor.Business.Contracts;
using DataProcessor.Business.Services;
using DataProcessor.DataAccess;
using NLog.Web;

namespace DataProcessor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IPersonProcessorService, PersonProcessorService>();

            builder.Services.AddDbContext<AppDbContext>();

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

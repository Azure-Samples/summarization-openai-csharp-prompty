
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using SummarizationAPI.Evaluations;
using SummarizationAPI.Summarization;

namespace SummarizationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<OpenAIClient>(serviceProvider =>
            {
                return new OpenAIClient(new Uri(builder.Configuration["OpenAi:endpoint"]), new DefaultAzureCredential());
            });

            builder.Services.AddScoped<SummarizationService>();
            builder.Services.AddScoped<Evaluation>();

            //Application Insights
            builder.Services.AddOpenTelemetry().UseAzureMonitor(options => {
                options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
                options.Credential = new DefaultAzureCredential();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

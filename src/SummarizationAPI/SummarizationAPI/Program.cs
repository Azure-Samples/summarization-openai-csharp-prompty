using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.SemanticKernel;
using SummarizationAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(_ => new OpenAIClient(new Uri(builder.Configuration["OpenAi:endpoint"]!), new DefaultAzureCredential()));
builder.Services.AddKernel().AddAzureOpenAIChatCompletion(builder.Configuration["OpenAi:deployment"]!);
builder.Services.AddScoped<SummarizationService>();

//// Application Insights
//builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
//{
//    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
//    options.Credential = new DefaultAzureCredential();
//});

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
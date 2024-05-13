using Microsoft.SemanticKernel;
using System.Text.Json;
using SummarizationAPI;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Summarization.Evaluation.Tests
{
    public class Evaluate
    {
        //create chatService and serviceProvider
        private readonly SummarizationService _summarizationService;
        private readonly ServiceProvider _serviceProvider;

        public Evaluate()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new OpenAIClient(new Uri(config["OpenAi:endpoint"]!), new DefaultAzureCredential()));
            serviceCollection.AddKernel();
            serviceCollection.AddAzureOpenAIChatCompletion(config["OpenAi:deployment"]!);
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<SummarizationService>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _summarizationService = _serviceProvider.GetRequiredService<SummarizationService>();
        }

        //Test EvaluationResult
        [Theory]
        [InlineData("I need to open a problem report for part number ABC123. The brake rotor is overheating causing glazing on the pads. We track temperature above 24 degrees Celsius and we are seeing this after three to four laps during runs when the driver is braking late and aggressively into corners. The issue severity is to be prioritized as a 2. This is impacting the front brake assembly EFG234")]
        public async void EvaluationResult(string problem)
        {

            // GetResponse from chat service
            var result = await _summarizationService.GetResponseAsync(problem);
            // parse result string varibales of context and answer
            var response = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(result);
            var summary = (response?["summary"]).ToString();
     


            //GetEvaluation from chat service
            var score = await _summarizationService.GetEvaluationAsync(problem, summary);

            var relevance = int.Parse(score["relevance"]);



            Assert.Multiple(
                () => Assert.True(relevance >= 3, $"Relevance of {problem} - score {relevance}, expecting min 3."));
        }

    }
}

using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Newtonsoft.Json.Linq;

namespace SummarizationAPI.Evaluations
{
    public class Evaluation
    {
        private readonly ILogger<Evaluation> _logger;
        private readonly OpenAIClient _openaiClient;
        private readonly string _deploymentName;

        public Evaluation(ILogger<Evaluation> logger, OpenAIClient openAiClient, IConfiguration config)
        {
            _logger = logger;
            _openaiClient = openAiClient;
            _deploymentName = config["OpenAi:deployment"];
        }

        // Run a batch coherence evaluation
        public async Task<List<string>> Batch(string file, string prompty)
        {
            if(!File.Exists(file))
            {
                file =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.jsonl");
            }

            var results = new List<string>();
            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                var data = JObject.Parse(line);
                var result = await Evaluate(data["problem"].ToString(), data["summary"].ToString(), prompty);
                results.Add(result);
            }

            return results;
        }

        // Run a single coherence evaluation
        public async Task<string> Evaluate(string problem, string summary, string prompty)
        {
            var kernel = Kernel.CreateBuilder()
                               .AddAzureOpenAIChatCompletion(_deploymentName, _openaiClient)
                               .Build();

            var cwd = Directory.GetCurrentDirectory();
            var chatPromptyPath = Path.Combine(cwd, prompty);

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var kernelFunction = kernel.CreateFunctionFromPrompty(chatPromptyPath);
#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            _logger.LogInformation("Getting result...");
            var arguments = new KernelArguments(){
                { "problem", problem },
                { "summary", summary }
            };

            var kernalResult = await kernelFunction.InvokeAsync(kernel, arguments);
            //get string result

            // Create score dict with results
            var message = kernalResult.ToString();

            return message;
        }
    }
}

using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Newtonsoft.Json.Linq;

namespace SummarizationAPI.Evaluations
{
    public static class Evaluation
    {
        // Run a batch coherence evaluation
        public static async Task<List<string>> Batch(string file, string prompty, string deploymentName, OpenAIClient client)
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
                var result = await Evaluate(data["problem"].ToString(), data["summary"].ToString(), prompty, deploymentName, client);
                results.Add(result);
            }

            return results;
        }

        // Run a single coherence evaluation
        public static async Task<string> Evaluate(string problem, string summary, string prompty, string deploymentName, OpenAIClient client)
        {
            var kernel = Kernel.CreateBuilder()
                               .AddAzureOpenAIChatCompletion(deploymentName, client)
                               .Build();

            var cwd = Directory.GetCurrentDirectory();
            var chatPromptyPath = Path.Combine(cwd, prompty);

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var kernelFunction = kernel.CreateFunctionFromPrompty(chatPromptyPath);
#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            Console.WriteLine("Getting result...");
            var arguments = new KernelArguments(){
                { "problem", problem },
                { "summary", summary }
            };

            var kernalResult = kernelFunction.InvokeAsync(kernel, arguments).Result;
            //get string result

            // Create score dict with results
            var message = kernalResult.ToString();

            return message;
        }
    }
}

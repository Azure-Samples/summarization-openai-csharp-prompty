
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using SummarizationAPI.Evaluations;
using Microsoft.SemanticKernel;
using Azure;

namespace SummarizationAPI.Summarization
{
    public class SummarizationService
    {
        private readonly IConfiguration _prompty;
        private readonly string _oaiEndpoint;
        private readonly string _oaiKey;

        private const string _deploymentName = "gpt-35-turbo";
        private readonly OpenAIClient _client;

        public SummarizationService()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

            _prompty = config.GetSection("prompty");

            _oaiEndpoint = _prompty["azure_endpoint"];
            _oaiKey = _prompty["api_key"];

            _client = new OpenAIClient(
                    endpoint: new Uri(_oaiEndpoint),
                    keyCredential: new AzureKeyCredential(_oaiKey));
        }

        public async Task<string> GetResponseAsync(string problem, List<string> chatHistory)
        {

            Console.WriteLine($"Inputs: Problem = {problem}");


            var kernel = Kernel.CreateBuilder()
                          .AddAzureOpenAIChatCompletion(_deploymentName, _client)
                          .Build();

            var cwd = Directory.GetCurrentDirectory();
            var chatPromptyPath = Path.Combine(cwd, "summarize.prompty");

            var kernelFunction = kernel.CreateFunctionFromPrompty(chatPromptyPath);

            Console.WriteLine("Getting result...");
            var arguments = new KernelArguments(){
                { "problem", problem }
            };

            var kernalResult = kernelFunction.InvokeAsync(kernel, arguments).Result;
            //get string result

            // Create score dict with results
            var score = new Dictionary<string, string>();
            var message = kernalResult.ToString();

            score["coherence"] = await Evaluation.Evaluate(problem, message, "./Evaluations/coherence.prompty", _deploymentName, _client);
            score["relevance"] = await Evaluation.Evaluate(problem, message, "./Evaluations/relevance.prompty", _deploymentName, _client);
            score["fluency"] = await Evaluation.Evaluate(problem, message, "./Evaluations/fluency.prompty", _deploymentName, _client);

            Console.WriteLine($"Result: {kernalResult}");
            Console.WriteLine($"Score: {string.Join(", ", score)}");
            // add score to result

            var result = JsonConvert.SerializeObject(new { message, score });

            return result;
        }
    }
}


using Azure.AI.OpenAI;
using Newtonsoft.Json;
using SummarizationAPI.Evaluations;
using Microsoft.SemanticKernel;
using Azure;

namespace SummarizationAPI.Summarization
{
    public class SummarizationService
    {
        private readonly Evaluation _evaluation;
        private readonly ILogger<SummarizationService> _logger;
        private readonly OpenAIClient _openaiClient;
        private readonly string _deploymentName;

        public SummarizationService(ILogger<SummarizationService> logger, OpenAIClient openaiClient, IConfiguration config, Evaluation evaluation)
        {
            _logger = logger;
            _openaiClient = openaiClient;
            _deploymentName = config["OpenAi:deployment"];
            _evaluation = evaluation;
        }

        public async Task<string> GetResponseAsync(string problem, List<string> chatHistory)
        {
            _logger.LogInformation($"Inputs: Problem = {problem}");

            var kernel = Kernel.CreateBuilder()
                          .AddAzureOpenAIChatCompletion(_deploymentName, _openaiClient)
                          .Build();

            var cwd = Directory.GetCurrentDirectory();
            var chatPromptyPath = Path.Combine(cwd, "summarize.prompty");

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var kernelFunction = kernel.CreateFunctionFromPrompty(chatPromptyPath);
#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            _logger.LogInformation("Getting result...");
            var arguments = new KernelArguments(){
                { "problem", problem }
            };

            var kernelResult = await kernelFunction.InvokeAsync(kernel, arguments);
            //get string result

            // Create score dict with results
            var score = new Dictionary<string, string>();
            var message = kernelResult.ToString();

            score["coherence"] = await _evaluation.Evaluate(problem, message, "./Evaluations/coherence.prompty");
            score["relevance"] = await _evaluation.Evaluate(problem, message, "./Evaluations/relevance.prompty");
            score["fluency"] = await _evaluation.Evaluate(problem, message, "./Evaluations/fluency.prompty");

            _logger.LogInformation($"Result: {kernelResult}");
            _logger.LogInformation($"Score: {string.Join(", ", score)}");
            // add score to result

            var result = JsonConvert.SerializeObject(new { message, score });

            return result;
        }
    }
}

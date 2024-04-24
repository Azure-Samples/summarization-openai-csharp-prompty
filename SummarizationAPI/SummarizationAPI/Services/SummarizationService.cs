
using Newtonsoft.Json;
using SummarizationAPI.Evaluations;

namespace SummarizationAPI.Summarization
{
    public class SummarizationService
    {
        private readonly IConfiguration _prompty;
        private readonly string _oaiEndpoint;
        private readonly string _oaiKey;

        public SummarizationService()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

            _prompty = config.GetSection("prompty");

            _oaiEndpoint = _prompty["azure_endpoint"];
            _oaiKey = _prompty["api_key"];

        }

        public async Task<string> GetResponseAsync(string problem, List<string> chatHistory)
        {

            Console.WriteLine($"Inputs: Problem = {problem}");


            var inputs = new Dictionary<string, dynamic>
            {
                { "problem", problem },
            };

            var prompty = new Prompty.Core.Prompty();
            prompty.Inputs = inputs;
            prompty = await prompty.Execute("summarize.prompty", prompty);
            var result = prompty.ChatResponseMessage.Content;

            // Create score dict with results
            var score = new Dictionary<string, string>();

            score["groundedness"] = await Evaluation.Evaluate(problem, problem, result, "./Evaluations/groundedness.prompty");
            score["coherence"] = await Evaluation.Evaluate(problem, problem, result, "./Evaluations/coherence.prompty");
            score["relevance"] = await Evaluation.Evaluate(problem, problem, result, "./Evaluations/relevance.prompty");
            score["fluency"] = await Evaluation.Evaluate(problem, problem, result, "./Evaluations/fluency.prompty");

            Console.WriteLine($"Result: {result}");
            //Console.WriteLine($"Score: {string.Join(", ", score)}");
            // add score to result
            result = JsonConvert.SerializeObject(new { result, score });

            return result;
        }
    }
}

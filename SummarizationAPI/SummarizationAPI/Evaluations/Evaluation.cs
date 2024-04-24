using Newtonsoft.Json.Linq;
using Prompty.Core;

namespace SummarizationAPI.Evaluations
{
    public static class Evaluation
    {
        // Run a batch coherence evaluation
        public static async Task<List<string>> Batch(string file, string path)
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
                var result = await Evaluate(data["problem"].ToString(), data["summary"].ToString(), path);
                results.Add(result);
            }

            return results;
        }

        // Run a single coherence evaluation
        public static async Task<string> Evaluate(string problem, string summary, string path)
        {
            var inputs = new Dictionary<string, dynamic>
            {
                { "problem", problem },
                { "summary", summary }
            };

            var prompty = new Prompty.Core.Prompty();
            prompty.Inputs = inputs;
            prompty = await prompty.Execute(path, prompty);
            var result = prompty.ChatResponseMessage.Content;

            // Replace this with your actual coherence evaluation logic
            // For demonstration purposes, I'll return a placeholder result.
            return result;
        }
    }
}

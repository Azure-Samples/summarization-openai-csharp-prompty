using Microsoft.SemanticKernel;
using System.Text.Json;

namespace SummarizationAPI;

public sealed class SummarizationService(Kernel kernel, ILogger<SummarizationService> logger)
{
    private readonly Kernel _kernel = kernel;
    private readonly ILogger<SummarizationService> _logger = logger;

    private readonly KernelFunction _summarize = kernel.CreateFunctionFromPromptyFile("summarize.prompty");
    private readonly KernelFunction _coherence = kernel.CreateFunctionFromPromptyFile(Path.Combine("Evaluations", "coherence.prompty"));
    private readonly KernelFunction _relevance = kernel.CreateFunctionFromPromptyFile(Path.Combine("Evaluations", "relevance.prompty"));
    private readonly KernelFunction _fluency = kernel.CreateFunctionFromPromptyFile(Path.Combine("Evaluations", "fluency.prompty"));

    public async Task<string> GetResponseAsync(string problem)
    {
        _logger.LogInformation("Getting summary for {Problem}", problem);
        var summary = await _summarize.InvokeAsync<string>(_kernel, new()
        {
            { "problem", problem }
        });

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Result: {Summary}", summary);
        }

        return JsonSerializer.Serialize(new { summary });
    }
    // Evaluate the answer using the specified function.
    public async Task<Dictionary<string, string?>> GetEvaluationAsync(string problem, string summary)
    {
        _logger.LogInformation("Evaluating result.");
        var coherenceEvaluation = Evaluate(_coherence, problem, summary);
        var relevanceEvaluation = Evaluate(_relevance, problem, summary);
        var fluencyEvaluation = Evaluate(_fluency, problem, summary);

        var score = new Dictionary<string, string?>
        {
            ["coherence"] = await coherenceEvaluation,
            ["relevance"] = await relevanceEvaluation,
            ["fluency"] = await fluencyEvaluation,
        };

        await Task.WhenAll(coherenceEvaluation, relevanceEvaluation, fluencyEvaluation);
        _logger.LogInformation("Score: {Score}", score);
        return score;
    }


    private Task<string?> Evaluate(KernelFunction func, string problem, string? summary)
    {
        return func.InvokeAsync<string>(_kernel, new()
        {
            { "problem", problem },
            { "summary", summary }
        });
    }
}

using Microsoft.SemanticKernel;
using System.Text.Json;

namespace SummarizationAPI.Summarization;

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

        var score = new Dictionary<string, string?>
        {
            ["coherence"] = await Evaluate(_coherence, problem, summary),
            ["relevance"] = await Evaluate(_relevance, problem, summary),
            ["fluency"] = await Evaluate(_fluency, problem, summary)
        };

        _logger.LogInformation("Summary: {Summary}", summary);
        _logger.LogInformation("Score: {Score}", score);
        return JsonSerializer.Serialize(new { summary, score });
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

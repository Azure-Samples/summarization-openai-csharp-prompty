using Microsoft.AspNetCore.Mvc;
using SummarizationAPI;

namespace SummarizationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class SummarizationController(SummarizationService summarizationService, ILogger<SummarizationController> logger) : ControllerBase
{
    [HttpPost(Name = "PostSummarizationRequest")]
    public async Task<string> Post(string problem)
    {
        string result = await summarizationService.GetResponseAsync(problem);
        logger.LogInformation("Result: {Result}", result);
        return result;
    }
}

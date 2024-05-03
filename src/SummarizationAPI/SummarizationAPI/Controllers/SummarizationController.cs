using Microsoft.AspNetCore.Mvc;
using SummarizationAPI.Summarization;

namespace SummarizationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SummarizationController : ControllerBase
    {
        private readonly ILogger<SummarizationController> _logger;
        private readonly SummarizationService summarizationServive;

        public SummarizationController(ILogger<SummarizationController> logger, SummarizationService summarizationServive)
        {
            _logger = logger;
            this.summarizationServive = summarizationServive;
        }


        [HttpPost(Name = "PostSummarizationRequest")]
        public async Task<string> Post(string problem, List<string> chatHistory)
        {
            string result = await summarizationServive.GetResponseAsync(problem, chatHistory.ToList());
            _logger.LogInformation(result);
            return result;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SummarizationAPI.Summarization;

namespace SummarizationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SummarizationController : ControllerBase
    {

        private readonly ILogger<SummarizationController> _logger;

        public SummarizationController(ILogger<SummarizationController> logger)
        {
            _logger = logger;
        }


        [HttpPost(Name = "PostSummarizationRequest")]
        public string Post(string problem, List<string> chatHistory)
        {
            var summarizationServive = new SummarizationService();
            string result = summarizationServive.GetResponseAsync(problem, chatHistory.ToList()).Result;
            Console.WriteLine(result);
            return result;
        }
    }
}

using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace SpeechRecognition.ViewModels
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            // Load configuration
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            SpeechResourceId = Configuration["AzureSpeech:ResourceId"];
            SpeechRegion = Configuration["AzureSpeech:Region"];
            BackendApi = Configuration["BackendApi"];

            var credentials = new DefaultAzureCredential();
            var context = new TokenRequestContext(new string[] { "https://cognitiveservices.azure.com/.default" });
            var defaultToken = credentials.GetToken(context);
            string aadToken = defaultToken.Token;
            AuthToken = $"aad#{SpeechResourceId}#{aadToken}";
        }



        public IConfigurationRoot Configuration { get; }
        public string  SpeechResourceId { get;}
        public string SpeechRegion { get; }
        public string BackendApi { get; }
        public string AuthToken { get; }
    }
}

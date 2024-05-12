using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

bool useSampleData = true; ; // Change if you want to use sample data instead of recording your voice

// Load configuration from appsettings.json from SummarizationAPI project
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string speechResourceId = config["AzureSpeech:ResourceId"];
string speechRegion = config["AzureSpeech:Region"];
string backendApi = config["BackendApi"];

if (String.IsNullOrEmpty(speechResourceId) || String.IsNullOrEmpty(speechRegion) || String.IsNullOrEmpty(backendApi))
{
    Console.WriteLine("Please set the following values in the appsettings.json file:");
    Console.WriteLine("AzureSpeech:ResourceId - Azure Speech Service resource ID");
    Console.WriteLine("AzureSpeech:Region - Region for the Azure Speech Service");
    Console.WriteLine("BackendApi - Backend API URL");
    Console.ReadKey();
    return;
}

// Authenticate with the Azure Speech Service using Microsoft Entra
// Learn more: https://learn.microsoft.com/azure/ai-services/speech-service/how-to-configure-azure-ad-auth?tabs=portal&pivots=programming-language-csharp
var credentials = new DefaultAzureCredential();
var context = new TokenRequestContext(new string[] { "https://cognitiveservices.azure.com/.default" });
var defaultToken = credentials.GetToken(context);
string aadToken = defaultToken.Token;
string authToken = $"aad#{speechResourceId}#{aadToken}";

var speechConfig = SpeechConfig.FromAuthorizationToken(authToken, speechRegion);
speechConfig.SpeechRecognitionLanguage = "en-US";

SpeechRecognitionResult speechRecognitionResult;

Console.ForegroundColor = ConsoleColor.Black;
Console.BackgroundColor = ConsoleColor.Yellow;
if (useSampleData)
{
    using var audioConfig = AudioConfig.FromWavFileInput("../../../data/audio-data/issue0.wav");
    using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

    Console.WriteLine("Converting from speech to text using a sample audio file.");

    speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
} else
{
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

    Console.WriteLine("Speak into your microphone to report an issue.");
    speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
}
Console.ResetColor();

await ProcessSpeechRecognitionResult(speechRecognitionResult);

// Await on user input to keep the console window open
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

async Task ProcessSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
{
    switch (speechRecognitionResult.Reason)
    {
        case ResultReason.RecognizedSpeech:
            Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");

            SummarizationResponse summaryResponse = await SummarizeText(speechRecognitionResult.Text);
            
            Console.WriteLine();
            if (summaryResponse is null)
            {
                Console.WriteLine("No summaryResponse results - did you pass an empty prompt?");
            }
            else if (summaryResponse.IsErrorResult)
            {
                Console.WriteLine($"Error: {summaryResponse.Summary}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Here's a summary of the ticket to create:");
                Console.ResetColor();
                Console.WriteLine(summaryResponse.Summary);
            }
            break;
        case ResultReason.NoMatch:
            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            break;
        case ResultReason.Canceled:
            var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

            if (cancellation.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
            }
            break;
    }
}

 async Task<SummarizationResponse> SummarizeText(string text)
{
    var httpClient = new HttpClient();
    var httpContent = new StringContent("", Encoding.UTF8, "text/plain");

    // Encode the text parameter for inclusion in a URL request
    var queryParam = WebUtility.UrlEncode(text);

    var response = await httpClient.PostAsync($"{backendApi}?problem={queryParam}", httpContent);
    
    if (response.IsSuccessStatusCode)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SummarizationResponse>(jsonResponse);

        return result;
    }
    else
    {
        return new()
        {
            IsErrorResult = true,
            Summary = $"Failed to summarize text - HTTP status code: {response.StatusCode}. Reason: {response.ReasonPhrase}"
        };
    }
}

class SummarizationResponse
{
    public bool IsErrorResult { get; set; } = false;

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("score")]
    public Dictionary<string, string> Score { get; set; }
}
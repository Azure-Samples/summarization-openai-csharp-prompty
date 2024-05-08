using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Text.Encodings.Web;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

// This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
string speechResourceId = Environment.GetEnvironmentVariable("AZURE_SPEECH__RESOURCE_ID");
string speechRegion = Environment.GetEnvironmentVariable("AZURE_SPEECH__REGION");

var credentials = new DefaultAzureCredential();
TokenRequestContext context = new Azure.Core.TokenRequestContext(new string[] { "https://cognitiveservices.azure.com/.default" });
var defaultToken = credentials.GetToken(context);
string aadToken = defaultToken.Token;

string authToken = $"aad#{speechResourceId}#{aadToken}";

var speechConfig = SpeechConfig.FromAuthorizationToken(authToken, speechRegion);
speechConfig.SpeechRecognitionLanguage = "en-US";

using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

Console.WriteLine("Speak into your microphone.");
var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
ProcessSpeechRecognitionResult(speechRecognitionResult);
Console.ReadLine();

static async void ProcessSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
{
    switch (speechRecognitionResult.Reason)
    {
        case ResultReason.RecognizedSpeech:
            Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");

            string? summary = await SummarizeText(speechRecognitionResult.Text);

            if (summary is null)
            {
                Console.WriteLine("Failed to summarize text - Couldn't not get response from backend service.");
            }
            else
            {
                Console.WriteLine($"SUMMARY: {summary}");
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

static async Task<string?> SummarizeText(string text)
{
    string backendApi = Environment.GetEnvironmentVariable("BACKEND_API");

    var httpClient = new HttpClient();
    var httpContent = new StringContent("", Encoding.UTF8, "text/plain");

    // Encode the text parameter for inclusion in a URL request
    var queryParam = WebUtility.UrlEncode(text);

    var response = await httpClient.PostAsync($"{backendApi}?problem={queryParam}", httpContent);
    
    if (response.IsSuccessStatusCode)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SummarizationResponse>(jsonResponse);

        return result.Summary;
    }
    else
    {
        return null;
    }
}

class SummarizationResponse
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("score")]
    public Dictionary<string, string> Score { get; set; }
}
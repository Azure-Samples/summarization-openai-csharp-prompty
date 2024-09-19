using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SummarizationAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SpeechRecognition.Model;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace SpeechRecognition.ViewModels
{
    public class SpeechToTextViewModel : BaseViewModel
    {
        private ServiceConfiguration _serviceConfiguration;
        private readonly SummarizationService _summarizationService;

        private bool _useMicrophone = true; // Default input option is microphone

        private ObservableCollection<SpeechToTextDataModel> _recognizedSpeechToText;
        public SpeechToTextViewModel(ServiceConfiguration config)
        {
            _serviceConfiguration = config;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new OpenAIClient(new Uri(_serviceConfiguration.Configuration["OpenAi:endpoint"]!), new DefaultAzureCredential()));
            serviceCollection.AddKernel();
            serviceCollection.AddAzureOpenAIChatCompletion(_serviceConfiguration.Configuration["OpenAi:deployment"]!);
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<SummarizationService>();

            // Build the service provider
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Get the SummarizationService from the service provider
            _summarizationService = serviceProvider.GetRequiredService<SummarizationService>();

            _recognizedSpeechToText = [];

        }

        public bool UseMicrophone
        {
            get => _useMicrophone;
            set
            {
                _useMicrophone = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SpeechToTextDataModel> RecognizedSpeechToText
        {
            get => _recognizedSpeechToText;
            set
            {
                _recognizedSpeechToText = value;
                OnPropertyChanged(nameof(RecognizedSpeechToText));
            }
        }

        public string SpeechFilePath { get; set; }


        public async Task SummerizeSpeech()
        {
            //this.IsCognitiveServicesToThink = true;
            //speechRecognizerInProgress.Invoke(new(), null);
            SpeechRecognitionResult result = await RecognizeSpeechAsync();
            switch (result?.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    var summary = await _summarizationService.GetResponseAsync(result.Text);
                    //RecognizedTexts.Add(result.Text);

                    if (summary is null)
                    {
                        RecognizedSpeechToText.Add(new SpeechToTextDataModel("No summaryResponse results - did you pass an empty prompt?"));
                    }
                    else
                    {
                        SummarizationResponse? summaryResponse = JsonSerializer.Deserialize<SummarizationResponse>(summary);
                        var evaluate = await _summarizationService.GetEvaluationAsync(result.Text, summaryResponse.Summary);
                        RecognizedSpeechToText.Add(new SpeechToTextDataModel(result.Text, summaryResponse.Summary, evaluate["relevance"]));
                    }
                    break;
                case ResultReason.NoMatch:
                    RecognizedSpeechToText.Add(new SpeechToTextDataModel($"NOMATCH: Speech could not be recognized."));
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    RecognizedSpeechToText.Add(new SpeechToTextDataModel($"CANCELED: Reason={cancellation.Reason}"));

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        RecognizedSpeechToText.Add(new SpeechToTextDataModel($"CANCELED: Reason={cancellation.Reason}"));
                        RecognizedSpeechToText.Add(new SpeechToTextDataModel($"CANCELED: ErrorDetails={cancellation.ErrorDetails}"));
                        RecognizedSpeechToText.Add(new SpeechToTextDataModel($"CANCELED: Did you set the speech resource key and region values?"));
                    }
                    break;
            }

            //IsCognitiveServicesToThink = false;
        }

        private async Task<SpeechRecognitionResult?> RecognizeSpeechAsync()
        {
            

            // Create a speech config with the specified subscription key and service region.
            var speechConfig = SpeechConfig.FromAuthorizationToken(_serviceConfiguration.AuthToken, _serviceConfiguration.SpeechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            SpeechRecognitionResult? speechRecognitionResult;

            if (UseMicrophone)
            {
                // Using microphone
                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();



                using var speechSynthesizer = new SpeechSynthesizer(speechConfig);



                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

                speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                return speechRecognitionResult;
            }
            else
            {
                // Using a file
                using var audioConfig = AudioConfig.FromWavFileInput(SpeechFilePath);
                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

                speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            }

            return speechRecognitionResult;
        }

        public void BrowseToAttachFile()
        {
            // Create an instance of OpenFileDialog
            OpenFileDialog openFileDialog = new()
            {
                // Set filter for file extension and default file extension
                DefaultExt = ".wav",
                Filter = "WAV Files (*.wav)|*.wav"
            };
            bool? result = openFileDialog.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                SpeechFilePath = openFileDialog.FileName;
                // You can now use the filename variable to process the selected file
            }
        }
    }
}

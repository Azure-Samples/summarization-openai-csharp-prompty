using Azure.Core;
using Azure.Identity;
using Microsoft.CognitiveServices.Speech;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechRecognition.ViewModels
{
    public class TextToSpeechViewModel : BaseViewModel
    {
        private ServiceConfiguration _serviceConfiguration;
        private ObservableCollection<byte[]> _audio;
        private string _textToSpeak;
        public TextToSpeechViewModel(ServiceConfiguration serviceConfiguration)
            : base()
        {
            _serviceConfiguration = serviceConfiguration;

            _audio = [];
            Audio.CollectionChanged += Audio_CollectionChanged;
        }


        public ObservableCollection<byte[]> Audio
        {
            get => _audio;
            set
            {
                _audio = value;
                OnPropertyChanged();
            }
        }

        public bool IsAudioGenerated => Audio.Count > 0;
            


        public bool IsAbleToConvertTextToSpeech
        {
            get => !string.IsNullOrEmpty(TextToSpeak);

        }

        public string TextToSpeak
        {
            get => _textToSpeak;
            set
            {
                _textToSpeak = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAbleToConvertTextToSpeech));
            }
        }
        private void Audio_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsAudioGenerated));
        }


        public async Task ProcessTextToSpeechAsync()
        {
            this.IsServiceStarted = true;
            this.OnServiceInProgress();

            // Create a speech config with the specified subscription key and service region.
            var speechConfig = SpeechConfig.FromAuthorizationToken(_serviceConfiguration.AuthToken, _serviceConfiguration.SpeechRegion);
            //speechConfig.SpeechRecognitionLanguage = "en-US";

            speechConfig.SpeechSynthesisVoiceName = "en-US-AvaMultilingualNeural";

            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);
            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            using var result = await speechSynthesizer.SpeakTextAsync(TextToSpeak);

            Audio.Add(result.AudioData);
            IsServiceStarted = false;
        }
    }
}

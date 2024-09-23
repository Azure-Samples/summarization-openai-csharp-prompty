using Microsoft.Identity.Client.NativeInterop;
using SpeechRecognition.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpeechRecognition
{
    public class MainViewModel : BaseViewModel
    {
        private bool _isSpeechToTextSelected;
        private ServiceConfiguration _serviceConfiguration;
        private event EventHandler _serviceInProgress;
        private bool _isServiceStarted;
        private string _serviceInProgressDots;
        public MainViewModel()
        {
            _isSpeechToTextSelected = true;
            _serviceConfiguration = new ServiceConfiguration();
            SpeechToTextViewModel = new SpeechToTextViewModel(_serviceConfiguration);
            TextToSpeechViewModel = new TextToSpeechViewModel(_serviceConfiguration);
            _serviceInProgress += OnServiceInProgress;
        }


        public SpeechToTextViewModel SpeechToTextViewModel { get; }

        public TextToSpeechViewModel TextToSpeechViewModel { get; } 

        public bool IsSpeechToTextSelected
        {
            get => _isSpeechToTextSelected;
            set
            {
                if (_isSpeechToTextSelected != value)
                {
                    _isSpeechToTextSelected = value;
                    OnPropertyChanged();
                }
            }
        }
     
    }
}

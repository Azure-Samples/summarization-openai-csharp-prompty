using SpeechRecognition.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpeechRecognition
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isSpeechToTextSelected;
        private ServiceConfiguration _serviceConfiguration;
        public MainViewModel()
        {
            _isSpeechToTextSelected = true;
            _serviceConfiguration = new ServiceConfiguration();
            SpeechToTextViewModel = new SpeechToTextViewModel(_serviceConfiguration);
            TextToSpeechViewModel = new TextToSpeechViewModel(_serviceConfiguration);
        }


        public SpeechToTextViewModel SpeechToTextViewModel { get; }

        public TextToSpeechViewModel TextToSpeechViewModel { get; } 

        public event PropertyChangedEventHandler? PropertyChanged;
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

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
    }
}

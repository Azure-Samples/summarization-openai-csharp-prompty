using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpeechRecognition.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
       
        private bool _isServiceStarted;
        private string _serviceInProgressDots;

        public BaseViewModel()
        {
            ServiceInProgress += OnServiceInProgress;
        }

        public event EventHandler ServiceInProgress;
        public bool IsServiceStarted
        {
            get => _isServiceStarted;
            set
            {
                if (_isServiceStarted != value)
                {
                    _isServiceStarted = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServiceInProgressDots
        {
            get => _serviceInProgressDots;
            set
            {
                if (_serviceInProgressDots != value)
                {
                    _serviceInProgressDots = value;
                    OnPropertyChanged();
                }
            }
        }

        // Method to invoke the event
        public void OnServiceInProgress()
        {
            ServiceInProgress?.Invoke(new(), new EventArgs());
        }
        public async void OnServiceInProgress(object sender, EventArgs e)
        {
            while (this.IsServiceStarted)
            {
                this.ServiceInProgressDots = string.Empty;
                for (int i = 0; i < 3; i++)
                {
                    if (!this.IsServiceStarted)
                    {
                        return;
                    }
                    this.ServiceInProgressDots = this.ServiceInProgressDots + ".";
                    await Task.Delay(500);
                }
            }
            return;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

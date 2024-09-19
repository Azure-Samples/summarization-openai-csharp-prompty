using SpeechRecognition.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpeechRecognition.UI
{
    /// <summary>
    /// Interaction logic for SpeechToTextUserControl.xaml
    /// </summary>
    public partial class SpeechToTextUserControl : UserControl
    {
        public SpeechToTextUserControl()
        {
            InitializeComponent();

        }

        private async void StartButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SpeechToTextViewModel viewModel)
            {
                await viewModel.SummerizeSpeech();
            }
        }

        private async void AttachFile_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SpeechToTextViewModel viewModel)
            {
                viewModel.BrowseToAttachFile();
                if (viewModel.SpeechFilePath != null)
                {
                    await viewModel.SummerizeSpeech();
                }

            }
        }
    }
}

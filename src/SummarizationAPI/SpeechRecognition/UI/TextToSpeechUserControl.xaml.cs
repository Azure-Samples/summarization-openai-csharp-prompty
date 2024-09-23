using NAudio.Wave;
using SpeechRecognition.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SpeechRecognition.UI
{
    /// <summary>
    /// Interaction logic for TextToSpeechUserControl.xaml
    /// </summary>
    public partial class TextToSpeechUserControl : UserControl
    {
        public TextToSpeechUserControl()
        {
            InitializeComponent();
        }

        private  async void SoundPlayer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get the DataContext of the item associated with this button
                var audioData = button.DataContext as byte[]; // Replace YourAudioClass with the actual type of the items in the Audio list

                if (audioData != null)
                {
                    // Handle your audio playback logic here
                    await PlayMp3FromBytesAsync(audioData);
                }
            }
        }

        private async Task PlayMp3FromBytesAsync(byte[] soundBytes)
        {
            using (var memoryStream = new MemoryStream(soundBytes))
            using (var mp3Reader = new Mp3FileReader(memoryStream))
            using (var waveOut = new WaveOutEvent())
            {
                waveOut.Init(mp3Reader);

                var tcs = new TaskCompletionSource<bool>();
              
                waveOut.Play();

                // Ensure the sound plays completely before disposing the resources
                await tcs.Task;
            }
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TextToSpeechViewModel viewModel)
            {
               
                await viewModel.ProcessTextToSpeechAsync();
            }
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {

            if (this.DataContext is TextToSpeechViewModel viewModel)
            {

                viewModel.Audio.Clear();
            }
        }
    }
}

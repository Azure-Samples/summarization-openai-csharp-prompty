
namespace SpeechRecognition.Model
{
    public class SpeechToTextDataModel
    {
        public SpeechToTextDataModel(string speechText, string summary, string speechSummaryEvaluationScore)
        {
            SpeechText = speechText;
            Summary = summary;
            SpeechSummaryEvaluationScore = speechSummaryEvaluationScore;
        }
        public SpeechToTextDataModel(string error) => ErrorMessage = error;
        public string SpeechText { get; set; }
        public string Summary { get; set; }

        public string SpeechSummaryEvaluationScore { get; set; }

        public string ErrorMessage { get; set; }
    }
}

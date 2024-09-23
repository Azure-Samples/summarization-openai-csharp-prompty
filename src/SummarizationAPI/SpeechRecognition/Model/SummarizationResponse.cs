using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeechRecognition.Model
{
    public class SummarizationResponse
    {
        public bool IsErrorResult { get; set; } = false;

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("score")]
        public Dictionary<string, string> Score { get; set; }
    }
}

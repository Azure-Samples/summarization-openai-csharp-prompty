using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prompty.Core
{
    public abstract class BaseModel
    {
        public string Prompt { get; set; }
        public List<Dictionary<string, string>> Messages { get; set; }
        public ChatResponseMessage ChatResponseMessage { get; set; }
        public Completions CompletionResponseMessage { get; set; }
        public Embeddings EmbeddingResponseMessage { get; set; }
        public ImageGenerations ImageResponseMessage { get; set; }
    }
}

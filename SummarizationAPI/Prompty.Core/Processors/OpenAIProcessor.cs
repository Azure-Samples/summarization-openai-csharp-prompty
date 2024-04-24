using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Prompty.Core.Types;

namespace Prompty.Core.Processors
{
    public class OpenAIProcessor : IInvoker
    {
        public OpenAIProcessor(Prompty prompty, InvokerFactory invoker)
        {
            invoker.Register(InvokerType.Processor, ProcessorType.openai.ToString(), this);
            invoker.Register(InvokerType.Processor, ProcessorType.azure.ToString(), this);
        }

        public async Task<BaseModel> Invoke(BaseModel data)
        {
            // parse chat response
            if (data.ChatResponseMessage != null)
            {
                var response = data.ChatResponseMessage;
                if (response.ToolCalls.Any())
                {
                    var result =  new SimpleModel<List<ChatCompletionsToolCall>>();
                    result.Item = (List<ChatCompletionsToolCall>)response.ToolCalls;
                    return result;
                }
                else
                {
                    var result = new SimpleModel<string>();
                    result.Item = response.Content;
                    return result;
                }
            }
            //process completion response
            if (data.CompletionResponseMessage != null)
            {
                var result = new SimpleModel<string>();
                result.Item = data.CompletionResponseMessage.Choices[0].Text;
                return result;
            }
            //process embedding response
            if (data.EmbeddingResponseMessage != null && data.EmbeddingResponseMessage.Data.Any())
            {
                var embeddings = new List<float>();
                foreach (var item in data.EmbeddingResponseMessage.Data)
                {
                    //TODO: fix embedding
                   // embeddings.Add(item.Embedding);
                }
                var result = new SimpleModel<List<float>>();
                result.Item = embeddings;
                return result;
            }
            // Throw exception if all results are null
            throw new ArgumentException("Invalid data type, unable to process result.");
        }
                
    }
}
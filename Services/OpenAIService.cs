using System.ClientModel;
using OpenAI.Chat;
using System.Net.Http.Headers;

namespace Hackathon_VAIT_New.Services
{
    public class OpenAIService
    {
        public readonly ChatClient client;

        public OpenAIService()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
        }

        public static readonly ChatTool GetJobDescriptionFromLink = ChatTool.CreateFunctionTool(
            functionName: "Web Search Job Description",
            functionDescription: "Get the job description from a provided link",
            functionParameters: BinaryData.FromBytes("""
                                                     {
                                                      "type": "web_search_preview",
                                                         "user_location": {
                                                           "type": "approximate",
                                                           "country": "AU"
                                                         },
                                                         "search_context_size": "medium"
                                                     }
                                                     """u8.ToArray())
        );

        public async Task<CollectionResult<StreamingChatCompletionUpdate>> StartChatMessage(List<ChatMessage> messages,
            ChatCompletionOptions? options)
        {
            return client.CompleteChatStreaming(messages, options);
        }
    }
}
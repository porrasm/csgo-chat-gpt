using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSGOChatGPT.Networking;

namespace CSGOChatGPT.ChatGPT {
    public static class ChatGPTApi {
        private static string endpoint = "https://api.openai.com/v1/chat/completions";

        public static async Task<ChatGPTResponse> GetResponse(ChatGPTRequest request) {
            Console.WriteLine("Executing ChatGPT request: " + request.messages[^1].content);
            try {
                ChatGPTResponse res = await Requests.PostJSON<ChatGPTRequest, ChatGPTResponse>(endpoint, request, Config.Instance.APIKey);
                return res;
            } catch (Exception e) {
                Console.WriteLine("Error in sending request: " + e);
            }

            return null;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ChatGPTMessage {
        public ChatGPTMessage(string role, string content) {
            this.role = role;
            this.content = content;
        }

        public string role { get; set; }
        public string content { get; set; }
    }

    public class ChatGPTRequest {
        public string model { get; set; } = "gpt-3.5-turbo";
        public List<ChatGPTMessage> messages { get; set; }

        public ChatGPTRequest(List<ChatGPTMessage> messages) {
            this.messages = messages;
        }

        public int? max_tokens { get; set; }

        public static ChatGPTRequest Default(string message, int responseTokens = 0) {
            ChatGPTRequest request = new ChatGPTRequest(new List<ChatGPTMessage>() { new ChatGPTMessage("user", message) });
            if (responseTokens > 0) {
                request.max_tokens = responseTokens;
            }

            return request;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ResponseChoice {
        public int index { get; set; }
        public ChatGPTMessage message { get; set; }
        public string finish_reason { get; set; }
    }

    public class ChatGPTResponse {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public List<ResponseChoice> choices { get; set; }
        public ResponseUsage usage { get; set; }

        public ChatGPTMessage GetFirstResponse() {
            if (choices == null || choices.Count == 0) {
                return null;
            }

            return choices[0].message;
        }
    }

    public class ResponseUsage {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}

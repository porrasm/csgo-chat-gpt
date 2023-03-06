using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT.ChatGPT {
    public class ChatGPTConversation {
        #region  fields
        private const string DEFAULT_INSTRUCTION = "You are a helpful assistant.";

        public bool IsGenerating { get; set; } = false;

        public int ChatHistoryLength { get; set; } = 5;
        public int ChatResponseTokenLength { get; set; } = 0;

        public List<ChatGPTMessage> Messages { get; set; } = new List<ChatGPTMessage>();
        public ChatGPTResponse LastResponse { get; set; }

        private ChatGPTMessage instructionMessage;
        #endregion

        public ChatGPTConversation(int chatHistoryLength = 12, string instruction = null) {
            this.ChatHistoryLength = chatHistoryLength;
            instructionMessage = new ChatGPTMessage("system", instruction ?? DEFAULT_INSTRUCTION);
        }

        public void AddMessage(string role, string content) {
            Messages.Add(new ChatGPTMessage(role, content));
            if (Messages.Count > ChatHistoryLength) {
                Messages.RemoveAt(1);
            }
        }

        public async Task<ChatGPTMessage> GenerateResponse(string newMessage) {
            if (IsGenerating) {
                throw new Exception("Wait for an AI response first");
            }

            AddMessage("user", newMessage);
            return await GenerateResponse();
        }

        public async Task<ChatGPTMessage> GenerateResponse() {
            if (IsGenerating) {
                throw new Exception("Wait for an AI response first");
            }

            IsGenerating = true;

            try {

                List<ChatGPTMessage> requestMessages = new List<ChatGPTMessage>();
                requestMessages.Add(instructionMessage);
                requestMessages.AddRange(Messages);

                ChatGPTRequest request = new ChatGPTRequest(requestMessages);
                if (ChatResponseTokenLength > 0) {
                    request.max_tokens = ChatResponseTokenLength;
                }

                LastResponse = await ChatGPTApi.GetResponse(request);

                if (LastResponse == null) {
                    Console.WriteLine("Error in sending request");
                    return null;
                }

                var message = LastResponse.GetFirstResponse();

                if (!Config.Instance.IgnoreAIMessagesInHistory) {
                    AddMessage(message.role, message.content);
                }

                return message;
            } catch (Exception e) {
                return null;
            } finally {
                WaitTimeout();
            }
        }

        private async Task WaitTimeout() {
            await Task.Delay(1500);
            IsGenerating = false;
        }
    }
}

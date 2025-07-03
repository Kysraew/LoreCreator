//from https://github.com/HamedFathi/YouTube/blob/main/CS-Ollama-StructuredOutput/OllamaStructuredOutput/ChatResponseExtensions.cs#L8

using OllamaSharp.Models.Chat;
using System.Text;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication.Ollama
{
  public static class ChatResponseExtensions
  {
    public static async Task<string> GetContentAsync(this IAsyncEnumerable<ChatResponseStream?> stream)
    {
      var responseBuilder = new StringBuilder();

      await foreach (var chunk in stream)
      {
        if (chunk is null)
          continue;

        var content = chunk.Message.Content;

        if (string.IsNullOrEmpty(content))
          continue;

        responseBuilder.Append(content);
        if (chunk.Done)
          break;
      }

      return responseBuilder.ToString();
    }
  }
}
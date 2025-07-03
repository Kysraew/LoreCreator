using OllamaSharp;
using LoreCreatorBackend.Models;
using OllamaSharp.Models.Chat;
using System.Text.Json;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication.Ollama
{


  public class OllamaProvider : ILlmProvider
  {

    private OllamaApiClient _client;
    private string _modelName;

    public OllamaProvider(string ollamaAddress, string modelName)
    {
      _client = new OllamaApiClient(ollamaAddress);
      _modelName = modelName;


    }

    public async Task<EntityDto> GetGeneratedEntityAsync(WorldSetting worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities)
    {

      var schema = new
      {
        type = "object",
        properties = new
        {
          name = new { type = "string" },
          description = new { type = "string" },
          entityTypes = new { type = "string" },
          relatedEntities = new
          {
            type = "array",
            items = new { type = "string" }
          }
        },
        required = new[] { "name", "description", "relatedEntities" },
        additionalProperties = false
      };

      var schemaNode = JsonSerializer.SerializeToNode(schema);

      string question = "";
      var chatRequest = new ChatRequest
      {
        Model = _modelName,
        Messages = new List<Message>
                    {
                        new() {
                            Role = ChatRole.User,
                            Content = question
                        }
                    },
        Format = schemaNode,
        Stream = false
      };

      var output = await _client.ChatAsync(chatRequest).GetContentAsync();

      EntityDto? createdEntity = JsonSerializer.Deserialize<EntityDto>(output);

      if (createdEntity == null) throw new LlmResponseWrongFromatException("Can't deserialize LLM output.");

      return createdEntity;
    }
  }
}
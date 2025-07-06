using OllamaSharp;
using LoreCreatorBackend.Models;
using OllamaSharp.Models.Chat;
using System.Text.Json;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication.Ollama
{


  public class OllamaProvider : ILlmProvider
  {

    private OllamaApiClient _client;
    public readonly string modelName = "qwen3:0.6b";
    public readonly string address = "127.0.0.1:11434";

    public OllamaProvider()
    {
      _client = new OllamaApiClient(address);


    }

    public async Task<EntityDto> GetGeneratedEntityAsync(EntityDto entityInitial, World worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities)
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
            items = new
            {
              type = "object",
              properties = new
              {
                name = new { type = "string" },
                entityType = new { type = "string" }
              },
              required = new[] { "name", "entityType" }
            }
          }
        },
        required = new[] { "name", "description", "entityType", "relatedEntities" },
        additionalProperties = false
      };

      var schemaNode = JsonSerializer.SerializeToNode(schema);

      string question = "";
      var chatRequest = new ChatRequest
      {
        Model = modelName,
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


    public async Task<EntityDto> GetUpdatedEntityAsync(EntityDto entityInitial, World worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities)
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
            items = new
            {
              type = "object",
              properties = new
              {
                name = new { type = "string" },
                entityType = new { type = "string" }
              },
              required = new[] { "name", "entityType" }
            }
          }
        },
        required = new[] { "name", "description", "entityType", "relatedEntities" },
        additionalProperties = false
      };

      var schemaNode = JsonSerializer.SerializeToNode(schema);

      string question = "";
      var chatRequest = new ChatRequest
      {
        Model = modelName,
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
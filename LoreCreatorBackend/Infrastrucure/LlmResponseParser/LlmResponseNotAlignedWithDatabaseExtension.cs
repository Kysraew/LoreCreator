using System;

namespace LoreCreatorBackend.Infrastrucure.LlmResponseParser
{
  public class LlmResponseNotAlignedWithDatabaseExtension : Exception
  {
    public LlmResponseNotAlignedWithDatabaseExtension()
    {
    }

    public LlmResponseNotAlignedWithDatabaseExtension(string message)
        : base(message)
    {
    }

    public LlmResponseNotAlignedWithDatabaseExtension(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}

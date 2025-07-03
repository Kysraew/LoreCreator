using System;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication
{
  public class LlmResponseWrongFromatException : Exception
  {
    public LlmResponseWrongFromatException()
    {
    }

    public LlmResponseWrongFromatException(string message)
        : base(message)
    {
    }

    public LlmResponseWrongFromatException(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}

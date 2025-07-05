using System;

namespace LoreCreatorBackend.Infrastrucure.DtoValidators
{
  public class EntityDtoInvalidException : Exception
  {
    public EntityDtoInvalidException()
    {
    }

    public EntityDtoInvalidException(string message)
        : base(message)
    {
    }

    public EntityDtoInvalidException(string message, Exception inner)
        : base(message, inner)
    {
    }
  }
}

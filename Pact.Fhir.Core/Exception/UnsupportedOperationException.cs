namespace Pact.Fhir.Core.Exception
{
  using System;

  /// <summary>
  /// Can be used for repository implementations, if
  /// a certain operation is not supported in that system
  /// </summary>
  public class UnsupportedOperationException : Exception
  {
    public UnsupportedOperationException(string operation)
      : base($"Operation '{operation}' is not supported")
    {
    }
  }
}
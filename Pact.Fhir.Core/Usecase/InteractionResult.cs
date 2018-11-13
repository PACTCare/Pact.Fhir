namespace Pact.Fhir.Core.Usecase
{
  /// <summary>
  /// The interaction result.
  /// </summary>
  public enum InteractionResult
  {
    /// <summary>
    /// The success.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The iota exception.
    /// </summary>
    IotaException = -1,

    /// <summary>
    /// The invalid bundle.
    /// </summary>
    InvalidBundle = -2,

    /// <summary>
    /// The unknown entity.
    /// </summary>
    UnknownEntity = -3,
  }
}
namespace Pact.Fhir.Core.Usecase.GetResource
{
  using Tangle.Net.Entity;

  /// <summary>
  /// The get patient data request.
  /// </summary>
  public class GetResourceRequest
  {
    /// <summary>
    /// Gets or sets the root.
    /// </summary>
    public Hash Root { get; set; }
  }
}
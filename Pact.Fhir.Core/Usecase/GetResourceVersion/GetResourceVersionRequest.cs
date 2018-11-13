namespace Pact.Fhir.Core.Usecase.GetResourceVersion
{
  using Pact.Fhir.Core.Usecase.GetResource;

  using Tangle.Net.Entity;

  /// <summary>
  /// The get resource version request.
  /// </summary>
  public class GetResourceVersionRequest : GetResourceRequest
  {
    /// <summary>
    /// Gets or sets the resource root.
    /// </summary>
    public Hash ResourceRoot { get; set; }
  }
}
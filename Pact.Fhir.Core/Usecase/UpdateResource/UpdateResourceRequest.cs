namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The update resource request.
  /// </summary>
  /// <typeparam name="T">
  /// The resource type.
  /// </typeparam>
  public class UpdateResourceRequest<T>
    where T : DomainResource
  {
    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public T Resource { get; set; }

    /// <summary>
    /// Gets or sets the resource subseed.
    /// </summary>
    public Seed ResourceSubseed { get; set; }
  }
}
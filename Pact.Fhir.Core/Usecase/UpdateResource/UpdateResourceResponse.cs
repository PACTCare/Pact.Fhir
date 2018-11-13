namespace Pact.Fhir.Core.Usecase.UpdateResource
{
  using Hl7.Fhir.Model;

  /// <summary>
  /// The update resource response.
  /// </summary>
  /// <typeparam name="T">
  /// The resource type.
  /// </typeparam>
  public class UpdateResourceResponse<T>
    where T : DomainResource
  {
    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public T Resource { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    public InteractionResult Result { get; set; }
  }
}
namespace Pact.Fhir.Core.Usecase.GetResource
{
  using Hl7.Fhir.Model;

  /// <summary>
  /// The get patient data response.
  /// </summary>
  /// <typeparam name="T">
  /// The fhir type
  /// </typeparam>
  public class GetResourceResponse<T>
    where T : DomainResource
  {
    /// <summary>
    ///   Gets or sets the patient data.
    /// </summary>
    public T Resource { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    public InteractionResult Result { get; set; }
  }
}
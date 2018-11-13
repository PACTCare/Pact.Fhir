namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The write resource response.
  /// </summary>
  /// <typeparam name="T">
  /// The resource type.
  /// </typeparam>
  public class CreateResourceResponse<T>
    where T : DomainResource
  {
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public MaskedAuthenticatedMessage Message { get; set; }

    /// <summary>
    /// Gets or sets the resource subseed.
    /// </summary>
    public Seed ResourceSubseed { get; set; }

    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public T Resource { get; set; }
  }
}
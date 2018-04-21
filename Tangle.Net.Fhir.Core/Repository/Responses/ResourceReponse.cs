namespace Tangle.Net.Fhir.Core.Repository.Responses
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Mam.Entity;

  /// <summary>
  /// The create patient reponse.
  /// </summary>
  /// <typeparam name="T">
  /// The ressource type
  /// </typeparam>
  public class ResourceReponse<T>
    where T : Base
  {
    /// <summary>
    /// Gets or sets the channel.
    /// </summary>
    public MamChannel Channel { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public MaskedAuthenticatedMessage Message { get; set; }

    /// <summary>
    /// Gets or sets the patient.
    /// </summary>
    public T Resource { get; set; }
  }
}
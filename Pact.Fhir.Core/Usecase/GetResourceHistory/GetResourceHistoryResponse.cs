namespace Pact.Fhir.Core.Usecase.GetResourceHistory
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  /// <summary>
  /// The get resource history response.
  /// </summary>
  /// <typeparam name="T">
  /// The resource type.
  /// </typeparam>
  public class GetResourceHistoryResponse<T>
    where T : DomainResource
  {
    /// <summary>
    /// Gets or sets the history.
    /// </summary>
    public List<T> History { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    public InteractionResult Result { get; set; }
  }
}
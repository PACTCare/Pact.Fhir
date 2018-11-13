namespace Pact.Fhir.Api.Presenters
{
  using System.Linq;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.GetResourceHistory;

  /// <summary>
  /// The get resource history presenter.
  /// </summary>
  public class GetResourceHistoryPresenter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResourceHistoryPresenter"/> class.
    /// </summary>
    /// <param name="serializer">
    /// The serializer.
    /// </param>
    public GetResourceHistoryPresenter(FhirJsonSerializer serializer)
    {
      this.Serializer = serializer;
    }

    /// <summary>
    /// Gets the serializer.
    /// </summary>
    private FhirJsonSerializer Serializer { get; }

    /// <summary>
    /// The present.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <typeparam name="T">
    /// The resource type
    /// </typeparam>
    /// <returns>
    /// The <see cref="IActionResult"/>.
    /// </returns>
    public IActionResult Present<T>(GetResourceHistoryResponse<T> response)
      where T : DomainResource
    {
      if (response.Result == InteractionResult.Success)
      {
        return new JsonResult(response.History.Select(resource => this.Serializer.SerializeToDocument(resource)).ToArray());
      }

      switch (response.Result)
      {
        case InteractionResult.UnknownEntity:
          return new NotFoundObjectResult(new { response.Result });
        default:
          return new BadRequestObjectResult(new { response.Result });
      }
    }
  }
}
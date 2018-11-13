namespace Pact.Fhir.Api.Presenters
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.GetResource;

  /// <summary>
  /// The get resource presenter.
  /// </summary>
  public class GetResourcePresenter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GetResourcePresenter"/> class.
    /// </summary>
    /// <param name="serializer">
    /// The serializer.
    /// </param>
    public GetResourcePresenter(FhirJsonSerializer serializer)
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
    /// The fhir type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="JsonResult"/>.
    /// </returns>
    public IActionResult Present<T>(GetResourceResponse<T> response)
      where T : DomainResource
    {
      if (response.Result == InteractionResult.Success)
      {
        return new JsonResult(this.Serializer.SerializeToDocument(response.Resource));
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
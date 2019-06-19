namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;

  /// <summary>
  /// see http://hl7.org/fhir/http.html#create
  /// </summary>
  public class CreateResourceInteractor : UsecaseInteractor<CreateResourceRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public CreateResourceInteractor(IFhirRepository repository, FhirJsonParser fhirParser, ISearchRepository searchRepository)
      : base(repository)
    {
      this.FhirParser = fhirParser;
      this.SearchRepository = searchRepository;
    }

    private FhirJsonParser FhirParser { get; }

    private ISearchRepository SearchRepository { get; }

    public override async Task<ResourceResponse> ExecuteAsync(CreateResourceRequest request)
    {
      try
      {
        var requestResource = this.FhirParser.Parse<Resource>(request.ResourceJson);
        var resource = await this.Repository.CreateResourceAsync(requestResource);

        // add newly created resource to the search repository so a system wide search for resources can be performed
        // see https://www.hl7.org/fhir/search.html for details on search
        await this.SearchRepository.AddResourceAsync(resource);
        return new ResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (FormatException exception)
      {
        return new ResourceResponse { Code = ResponseCode.UnprocessableEntity, ExceptionMessage = exception.Message };
      }
      catch (Exception)
      {
        return new ResourceResponse
        {
                   Code = ResponseCode.Failure, ExceptionMessage = "Given resource was not processed. Please take a look at internal logs."
                 };
      }
    }
  }
}
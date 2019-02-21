namespace Pact.Fhir.Core.Usecase.PatchResource
{
  using System;
  using System.Threading.Tasks;

  using Pact.Fhir.Core.Entity;
  using Pact.Fhir.Core.Exception;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;

  public class PatchResourceInteractor : UsecaseInteractor<PatchResourceRequest, ResourceResponse>
  {
    /// <inheritdoc />
    public PatchResourceInteractor(IFhirRepository repository, IPatchApplier patchApplier)
      : base(repository)
    {
      this.PatchApplier = patchApplier;
    }

    private IPatchApplier PatchApplier { get; }

    /// <inheritdoc />
    public override async Task<ResourceResponse> ExecuteAsync(PatchResourceRequest request)
    {
      try
      {
        var resource = await this.Repository.ReadResourceAsync(request.ResourceId);

        resource = this.PatchApplier.ApplyTo(resource, PatchOperation.Parse(request.Payload));
        resource = await this.Repository.UpdateResourceAsync(resource);

        return new ResourceResponse { Code = ResponseCode.Success, Resource = resource };
      }
      catch (ResourceNotFoundException exception)
      {
        return new ResourceResponse { Code = ResponseCode.ResourceNotFound, ExceptionMessage = exception.Message };
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
namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System;

  using Pact.Fhir.Core.Repository;

  public class CreateResourceInteractor : UsecaseInteractor<CreateResourceRequest, CreateResourceResponse>
  {
    /// <inheritdoc />
    public CreateResourceInteractor(IFhirRepository repository)
      : base(repository)
    {
    }

    public override CreateResourceResponse Execute(CreateResourceRequest request)
    {
      this.Repository.CreateResource(request.Resource);

      return new CreateResourceResponse { Code = ResponseCode.Success };
    }
  }
}
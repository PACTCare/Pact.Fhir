namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Entity;

  /// <summary>
  /// The write resource interactor.
  /// </summary>
  public class CreateResourceInteractor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateResourceInteractor"/> class.
    /// </summary>
    /// <param name="fhirRepository">
    /// The fhir repository.
    /// </param>
    /// <param name="signingHelper">
    /// The signing helper.
    /// </param>
    public CreateResourceInteractor(IFhirPatientRepository fhirRepository, ISigningHelper signingHelper)
    {
      this.FhirRepository = fhirRepository;
      this.SigningHelper = signingHelper;
    }

    /// <summary>
    /// Gets the fhir repository.
    /// </summary>
    private IFhirPatientRepository FhirRepository { get; }

    /// <summary>
    /// Gets the signing helper.
    /// </summary>
    private ISigningHelper SigningHelper { get; }

    /// <summary>
    /// The execute async.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Task{TResult}"/>.
    /// </returns>
    public async Task<CreateResourceResponse<T>> ExecuteAsync<T>(CreateResourceRequest<T> request)
      where T : DomainResource
    {
      var subseed = await this.GetSubseed(request.Seed, request.Resource);
      var resourceReponse = await this.FhirRepository.CreateResourceAsync(request.Resource, subseed, request.ChannelKey);

      return new CreateResourceResponse<T> { Message = resourceReponse.Message, Resource = resourceReponse.Resource, ResourceSubseed = subseed };
    }

    /// <summary>
    /// The get subseed.
    /// </summary>
    /// <param name="seed">
    /// The seed.
    /// </param>
    /// <param name="resource">
    /// The resource.
    /// </param>
    /// <typeparam name="T">
    /// The resource type.
    /// </typeparam>
    /// <returns>
    /// The <see cref="Seed"/>.
    /// </returns>
    private async Task<Seed> GetSubseed<T>(Seed seed, T resource)
      where T : DomainResource
    {
      if (resource is Patient)
      {
        return new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(seed, StreamIndices.Patient)));
      }

      var index = StreamIndices.MedicationAdministrationStart;
      while (true)
      {
        var resourceChannelSubseed = new Seed(Converter.TritsToTrytes(this.SigningHelper.GetSubseed(seed, index)));

        if (!await this.FhirRepository.HasChannel(resourceChannelSubseed))
        {
          return resourceChannelSubseed;
        }

        index++;
      }
    }
  }
}
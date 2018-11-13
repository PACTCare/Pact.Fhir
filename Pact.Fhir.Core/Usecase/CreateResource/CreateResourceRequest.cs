namespace Pact.Fhir.Core.Usecase.CreateResource
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The write resource request.
  /// </summary>
  /// <typeparam name="T">
  /// The resource type.
  /// </typeparam>
  public class CreateResourceRequest<T>
    where T : DomainResource
  {
    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public T Resource { get; set; }

    /// <summary>
    /// Gets or sets the seed.
    /// </summary>
    public Seed Seed { get; set; }

    /// <summary>
    /// Gets or sets the channel key.
    /// </summary>
    public TryteString ChannelKey { get; set; }
  }
}
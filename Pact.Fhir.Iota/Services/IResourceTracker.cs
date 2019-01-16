namespace Pact.Fhir.Iota.Services
{
  using System.Threading.Tasks;

  using Pact.Fhir.Iota.Entity;

  /// <summary>
  /// Since the restricted MAM mode is used, we need to keep track of the channel keys belonging to certain roots
  /// </summary>
  public interface IResourceTracker
  {
    Task AddEntryAsync(ResourceEntry entry);

    Task<ResourceEntry> GetEntryAsync(string versionId);
  }
}
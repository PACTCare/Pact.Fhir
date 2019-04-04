namespace Pact.Fhir.Iota.Tests.Services
{
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;

  public class InMemoryReferenceResolver : IReferenceResolver
  {
    /// <inheritdoc />
    public void AddReference(string reference, Seed seed)
    {
    }

    /// <inheritdoc />
    public Seed Resolve(string reference)
    {
      return Seed.Random();
    }
  }
}
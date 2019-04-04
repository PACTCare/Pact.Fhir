namespace Pact.Fhir.Iota.Services
{
  using Tangle.Net.Entity;

  public interface IReferenceResolver
  {
    void AddReference(string reference, Seed seed);

    Seed Resolve(string reference);
  }
}
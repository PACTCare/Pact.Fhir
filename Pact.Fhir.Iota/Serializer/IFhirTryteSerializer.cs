namespace Pact.Fhir.Iota.Serializer
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  public interface IFhirTryteSerializer
  {
    T Deserialize<T>(TryteString value)
      where T : Base;

    TryteString Serialize<T>(T value)
      where T : Base;
  }
}
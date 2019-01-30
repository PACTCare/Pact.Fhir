namespace Pact.Fhir.Iota.Serializer
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Tangle.Net.Entity;

  public class FhirJsonTryteSerializer : IFhirTryteSerializer
  {
    public FhirJsonTryteSerializer()
    {
      this.Parser = new FhirJsonParser();
      this.Serializer = new FhirJsonSerializer();
    }

    public FhirJsonTryteSerializer(ParserSettings parserSettings, SerializerSettings serializerSettings)
    {
      this.Parser = new FhirJsonParser(parserSettings);
      this.Serializer = new FhirJsonSerializer(serializerSettings);
    }

    private FhirJsonParser Parser { get; }

    private FhirJsonSerializer Serializer { get; }

    /// <inheritdoc />
    public T Deserialize<T>(TryteString value)
      where T : Base
    {
      return this.Parser.Parse<T>(value.ToUtf8String());
    }

    /// <inheritdoc />
    public TryteString Serialize<T>(T value)
      where T : Base
    {
      return TryteString.FromUtf8String(this.Serializer.SerializeToString(value));
    }
  }
}
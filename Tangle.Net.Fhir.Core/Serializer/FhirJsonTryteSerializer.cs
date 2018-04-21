namespace Tangle.Net.Fhir.Core.Serializer
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Tangle.Net.Entity;

  /// <summary>
  /// The fhir tryte serializer.
  /// </summary>
  public class FhirJsonTryteSerializer : IFhirTryteSerializer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirJsonTryteSerializer"/> class.
    /// </summary>
    public FhirJsonTryteSerializer()
    {
      this.Parser = new FhirJsonParser();
      this.Serializer = new FhirJsonSerializer();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirJsonTryteSerializer"/> class.
    /// </summary>
    /// <param name="settings">
    /// The settings.
    /// </param>
    public FhirJsonTryteSerializer(ParserSettings settings)
    {
      this.Parser = new FhirJsonParser(settings);
      this.Serializer = new FhirJsonSerializer(settings);
    }

    /// <summary>
    /// Gets the parser.
    /// </summary>
    private FhirJsonParser Parser { get; }

    /// <summary>
    /// Gets the serializer.
    /// </summary>
    private FhirJsonSerializer Serializer { get; }

    /// <inheritdoc />
    public TryteString Serialize<T>(T value)
      where T : Base
    {
      return TryteString.FromUtf8String(this.Serializer.SerializeToString(value));
    }

    /// <inheritdoc />
    public T Deserialize<T>(TryteString value)
      where T : Base
    {
      return this.Parser.Parse<T>(value.ToUtf8String());
    }
  }
}
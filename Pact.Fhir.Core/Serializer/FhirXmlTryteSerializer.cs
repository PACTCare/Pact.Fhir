namespace Pact.Fhir.Core.Serializer
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Tangle.Net.Entity;

  /// <summary>
  /// The fhir xml tryte serializer.
  /// </summary>
  public class FhirXmlTryteSerializer : IFhirTryteSerializer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirXmlTryteSerializer"/> class. 
    /// </summary>
    public FhirXmlTryteSerializer()
    {
      this.Parser = new FhirXmlParser();
      this.Serializer = new FhirXmlSerializer();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirXmlTryteSerializer"/> class. 
    /// </summary>
    /// <param name="settings">
    /// The settings.
    /// </param>
    public FhirXmlTryteSerializer(ParserSettings settings)
    {
      this.Parser = new FhirXmlParser(settings);
      this.Serializer = new FhirXmlSerializer(settings);
    }

    /// <summary>
    /// Gets the parser.
    /// </summary>
    private FhirXmlParser Parser { get; }

    /// <summary>
    /// Gets the serializer.
    /// </summary>
    private FhirXmlSerializer Serializer { get; }

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
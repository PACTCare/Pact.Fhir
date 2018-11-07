namespace Pact.Fhir.Core.Serializer
{
  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  /// <summary>
  /// The FhirTryteSerializer interface.
  /// </summary>
  public interface IFhirTryteSerializer
  {
    /// <summary>
    /// The deserialize.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// The model to deserialize
    /// </typeparam>
    /// <returns>
    /// The <see cref="T"/>.
    /// </returns>
    T Deserialize<T>(TryteString value)
      where T : Base;

    /// <summary>
    /// The serialize.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// The model to serialize
    /// </typeparam>
    /// <returns>
    /// The <see cref="TryteString"/>.
    /// </returns>
    TryteString Serialize<T>(T value)
      where T : Base;
  }
}
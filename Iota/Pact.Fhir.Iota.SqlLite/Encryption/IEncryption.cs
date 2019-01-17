namespace Pact.Fhir.Iota.SqlLite.Encryption
{
  public interface IEncryption
  {
    string Decrypt(string data);

    string Encrypt(string data);
  }
}
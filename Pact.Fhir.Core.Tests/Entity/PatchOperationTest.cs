namespace Pact.Fhir.Core.Tests.Entity
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Newtonsoft.Json;

  using Pact.Fhir.Core.Entity;

  [TestClass]
  public class PatchOperationTest
  {
    [TestMethod]
    [ExpectedException(typeof(JsonSerializationException))]
    public void TestMalformedJsonInputShouldThrowException()
    {
      PatchOperation.Parse("[{ \"opsss\": \"test\", \"path\": \"/a/b/c\", \"value\": \"foo\" }]");
    }

    [TestMethod]
    public void TestJsonArrayCanBeParsed()
    {
      var result = PatchOperation.Parse(
        "[{ \"op\": \"test\", \"path\": \"/a/b/c\", \"value\": \"foo\" }, { \"op\": \"remove\", \"path\": \"/a/b/c\" }]");

      Assert.AreEqual(2, result.Count);

      Assert.AreEqual("test", result[0].Operation);
      Assert.AreEqual("/a/b/c", result[0].Path);
      Assert.AreEqual("foo", result[0].Value);
    }
  }
}
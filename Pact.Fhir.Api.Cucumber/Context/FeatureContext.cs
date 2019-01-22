namespace Pact.Fhir.Api.Cucumber.Context
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Net;

  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  using TechTalk.SpecFlow;

  public class FeatureContext
  {
    protected Process IisProcess { get; set; }

    protected string LastResponseBody { get; set; }

    protected HttpStatusCode LastResponseCode { get; set; }

    protected WebHeaderCollection LastResponseHeaders { get; set; }

    [BeforeScenario]
    public void StartIis()
    {
      try
      {
        this.CallApi("#");
      }
      catch (WebException)
      {
        var applicationPath = GetApplicationPath("Pact.Fhir.Api");
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        this.IisProcess = new Process
                            {
                              StartInfo =
                                {
                                  FileName = programFiles + @"\IIS Express\iisexpress.exe", Arguments = $"/path:{applicationPath} /port:64264"
                                }
                            };

        this.IisProcess.Start();
      }
      catch (Exception)
      {
        // ignored
      }
    }

    [AfterScenario]
    public void StopIis()
    {
      if (this.IisProcess != null && !this.IisProcess.HasExited)
      {
        this.IisProcess.Kill();
      }
    }

    protected void CallApi(string uri, string requestBody = "", string httpMethod = "POST", WebHeaderCollection headers = null)
    {
      var request = (HttpWebRequest)WebRequest.Create($"http://localhost:64264/{uri}");

      request.Method = httpMethod;
      request.ContentType = "application/fhir+json";
      request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

      if (headers != null)
      {
        request.Headers = headers;
      }

      if (httpMethod != "GET")
      {
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(requestBody);
        }
      }

      try
      {
        using (var response = request.GetResponse())
        {
          this.ParseResponse(response);
        }
      }
      catch (WebException exception)
      {
        using (var response = exception.Response)
        {
          this.ParseResponse(response);
        }
      }
    }

    protected bool IsValidJson(string stringValue)
    {
      if (string.IsNullOrWhiteSpace(stringValue))
      {
        return false;
      }

      var value = stringValue.Trim();

      if (value.StartsWith("{") && value.EndsWith("}") || // For object
          value.StartsWith("[") && value.EndsWith("]"))
      {
        // For array
        try
        {
          var obj = JToken.Parse(value);
          return true;
        }
        catch (JsonReaderException)
        {
          return false;
        }
      }

      return false;
    }

    private static string GetApplicationPath(string applicationPath)
    {
      var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
      return solutionFolder != null ? Path.Combine(solutionFolder, applicationPath) : string.Empty;
    }

    private void ParseResponse(WebResponse response)
    {
      using (var responseStream = response.GetResponseStream())
      {
        if (responseStream == null)
        {
          throw new Exception("Empty Response");
        }

        this.LastResponseCode = ((HttpWebResponse)response).StatusCode;
        this.LastResponseHeaders = ((HttpWebResponse)response).Headers;

        using (var reader = new StreamReader(responseStream))
        {
          this.LastResponseBody = reader.ReadToEnd();
        }
      }
    }
  }
}
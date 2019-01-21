namespace Pact.Fhir.Api.Cucumber.Context
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Net;
  using System.Text;

  using TechTalk.SpecFlow;

  public class FeatureContext
  {
    protected Process IisProcess { get; set; }

    protected HttpStatusCode LastHttpStatus { get; set; }

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
                                  FileName = programFiles + @"\IIS Express\iisexpress.exe", Arguments = $"/path:{applicationPath} /port:2020"
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

    private static string GetApplicationPath(string applicationPath)
    {
      var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
      return solutionFolder != null ? Path.Combine(solutionFolder, applicationPath) : string.Empty;
    }

    private string CallApi(string uri, string requestBody = "", string httpMethod = "POST")
    {
      var request = (HttpWebRequest)WebRequest.Create($"http://localhost:2020/{uri}");

      request.Method = httpMethod;
      request.ContentType = "application/fhir+json; charset=utf-8";

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
          using (var responseStream = response.GetResponseStream())
          {
            if (responseStream == null)
            {
              throw new Exception("Empty Response");
            }

            this.LastHttpStatus = ((HttpWebResponse)response).StatusCode;

            using (var reader = new StreamReader(responseStream))
            {
              return reader.ReadToEnd();
            }
          }
        }
      }
      catch (WebException exception)
      {
        using (var response = exception.Response)
        {
          using (var responseStream = response.GetResponseStream())
          {
            if (responseStream == null)
            {
              throw new Exception("Empty Response");
            }

            this.LastHttpStatus = ((HttpWebResponse)response).StatusCode;

            using (var reader = new StreamReader(responseStream))
            {
              return reader.ReadToEnd();
            }
          }
        }
      }
    }
  }
}
namespace Pact.Fhir.Api
{
  using Microsoft.AspNetCore;
  using Microsoft.AspNetCore.Hosting;

  /// <summary>
  /// The program.
  /// </summary>
  public class Program
  {
    /// <summary>
    /// The build web host.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// The <see cref="IWebHost"/>.
    /// </returns>
    public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();

    /// <summary>
    /// The main.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }
  }
}
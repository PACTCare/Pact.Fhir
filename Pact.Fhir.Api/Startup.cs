namespace Pact.Fhir.Api
{
  using System.Collections.Generic;

  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  using Pact.Fhir.Api.Services;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.SqlLite.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.SearchResources;
  using Pact.Fhir.Core.Usecase.ValidateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Iota.SqlLite.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository.Client;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddCors(
        options => options.AddPolicy("Development", builder => { builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod(); }));

      this.InjectDependencies(services);
    }

    private void InjectDependencies(IServiceCollection services)
    {
      var iotaRepository = new CachedIotaRestRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWSrvService());

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var encryption = new RijndaelEncryption("somenicekey", "somenicesalt");
      var fhirRepository = new IotaFhirRepository(
        iotaRepository,
        new FhirJsonTryteSerializer(),
        new SqlLiteResourceTracker(channelFactory, subscriptionFactory, encryption),
        new SqlLiteDeterministicCredentialProvider(
          new SqlLiteResourceTracker(channelFactory, subscriptionFactory, encryption),
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          iotaRepository),
        new SqlLiteReferenceResolver(encryption));
      var fhirParser = new FhirJsonParser();
      var searchRepository = new SqlLiteSearchRepository(fhirParser);

      var createInteractor = new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository);
      var readInteractor = new ReadResourceInteractor(fhirRepository);
      var capabilitiesInteractor = new GetCapabilitiesInteractor(fhirRepository, new AppConfigSystemInformation(this.Configuration));
      var validationInteractor = new ValidateResourceInteractor(fhirRepository, fhirParser);
      var searchInteractor = new SearchResourcesInteractor(fhirRepository, searchRepository);

      services.AddSingleton(createInteractor);
      services.AddSingleton(readInteractor);
      services.AddSingleton(capabilitiesInteractor);
      services.AddSingleton(validationInteractor);
      services.AddSingleton(searchInteractor);
    }
  }
}
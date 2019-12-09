using Pact.Fhir.Iota.Services;

namespace Pact.Fhir.Api
{
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;

  using Hl7.Fhir.Serialization;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  using Pact.Fhir.Api.Services;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.SqlLite.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.DeleteResource;
  using Pact.Fhir.Core.Usecase.GetCapabilities;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.ReadResourceHistory;
  using Pact.Fhir.Core.Usecase.ReadResourceVersion;
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
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository.Client;

  using MemoryCache = System.Runtime.Caching.MemoryCache;

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
      //if (env.IsDevelopment())
      //{
      //  app.UseDeveloperExceptionPage();
      //}
      //else
      //{
      //  app.UseHsts();
      //}

      app.UseDeveloperExceptionPage();
      app.UseHttpsRedirection();
      app.UseMvc();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMemoryCache();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddXmlDataContractSerializerFormatters();
      services.AddCors(
        options => options.AddPolicy("Development", builder => { builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod(); }));

      var extensionPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Extensions";
      if (!Directory.Exists(extensionPath))
      {
        Directory.CreateDirectory(extensionPath);
      }

      foreach (var file in Directory.GetFiles(extensionPath, "*.dll"))
      {
        services.AddMvc()
          .AddApplicationPart(Assembly.LoadFile(file))
          .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      }

      this.InjectDependencies(services);
    }

    private void InjectDependencies(IServiceCollection services)
    {
      var iotaRepository = new CachedIotaRestRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWService(new CpuPearlDiver()));

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var encryption = new RijndaelEncryption("somenicekey", "somenicesalt");
      var resourceTracker = new SqlLiteResourceTracker(channelFactory, subscriptionFactory, encryption);

      var seedManager = new SqlLiteDeterministicSeedManager(
        resourceTracker,
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        iotaRepository,
        encryption);

      var fhirRepository = new IotaFhirRepository(iotaRepository, new FhirJsonTryteSerializer(), resourceTracker, seedManager);
      var fhirParser = new FhirJsonParser();
      var searchRepository = new SqlLiteSearchRepository(fhirParser);

      var createInteractor = new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository);
      var readInteractor = new ReadResourceInteractor(fhirRepository, searchRepository);
      var readVersionInteractor = new ReadResourceVersionInteractor(fhirRepository);
      var readHistoryInteractor = new ReadResourceHistoryInteractor(fhirRepository);
      var capabilitiesInteractor = new GetCapabilitiesInteractor(fhirRepository, new AppConfigSystemInformation(this.Configuration));
      var validationInteractor = new ValidateResourceInteractor(fhirRepository, fhirParser);
      var searchInteractor = new SearchResourcesInteractor(fhirRepository, searchRepository);
      var deleteInteractor = new DeleteResourceInteractor(fhirRepository, searchRepository);

      var resourceImporter = new ResourceImporter(searchRepository, fhirRepository, seedManager);

      services.AddScoped(provider => createInteractor);
      services.AddScoped(provider => readInteractor);
      services.AddScoped(provider => capabilitiesInteractor);
      services.AddScoped(provider => validationInteractor);
      services.AddScoped(provider => searchInteractor);
      services.AddScoped(provider => resourceImporter);
      services.AddScoped(provider => readVersionInteractor);
      services.AddScoped(provider => readHistoryInteractor);
      services.AddScoped(provider => deleteInteractor);

      services.AddScoped<ISeedManager>(provider => seedManager);
      services.AddSingleton<IMemoryCache>(new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions()));
    }
  }
}
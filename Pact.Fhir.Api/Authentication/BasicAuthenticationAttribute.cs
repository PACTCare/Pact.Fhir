namespace Pact.Fhir.Api.Authentication
{
  using System;

  using Microsoft.AspNetCore.Mvc;

  /// <summary>
  /// The basic authorize attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class BasicAuthenticationAttribute : TypeFilterAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthenticationAttribute"/> class.
    /// </summary>
    public BasicAuthenticationAttribute()
      : base(typeof(BasicAuthenticationFilter))
    {
    }
  }
}
namespace Pact.Fhir.Api.Authentication
{
  using System;
  using System.Text;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Filters;

  /// <summary>
  /// The basic authentication filter.
  /// </summary>
  public class BasicAuthenticationFilter : IAuthorizationFilter
  {
    /// <inheritdoc />
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      string authHeader = context.HttpContext.Request.Headers["Authorization"];
      if (authHeader != null && authHeader.StartsWith("Basic "))
      {
        var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

        var username = decodedUsernamePassword.Split(':', 2)[0];
        var password = decodedUsernamePassword.Split(':', 2)[1];

        if (IsAuthorized(username, password))
        {
          return;
        }
      }

      context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
      context.Result = new UnauthorizedResult();
    }

    /// <summary>
    /// The is authorized.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private static bool IsAuthorized(string username, string password)
    {
      return username.Equals("", StringComparison.InvariantCultureIgnoreCase) && password.Equals("");
    }
  }
}
namespace AppCode.Extensions.GoogleRecaptchaV3
{
  /// <summary>
  /// Extension methods for HttpRequest to help with getting client IP
  /// this ensures that the code stays simple, but everything still works in both .NET Framework and .NET Core
  /// </summary>
  internal static class HttpRequestHelper
  {
#if NETCOREAPP

    public static string GetClientIp(this Microsoft.AspNetCore.Http.HttpRequest request)
    {
      return request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public static string GetHost(this Microsoft.AspNetCore.Http.HttpRequest request)
    {
      return request?.Host.Host;
    }

#else

    public static string GetClientIp(this System.Net.Http.HttpRequestMessage request)
    {
      return request.Properties.TryGetValue("MS_HttpContext", out var ctxObj)
        ? (ctxObj as System.Web.HttpContextBase).Request.UserHostAddress
        : null;
    }

    public static string GetHost(this System.Net.Http.HttpRequestMessage request)
    {
      return request.Properties.TryGetValue("MS_HttpContext", out var ctxObj)
        ? (ctxObj as System.Web.HttpContextBase).Request.Url?.Host
        : null;
    }

#endif
  }
}

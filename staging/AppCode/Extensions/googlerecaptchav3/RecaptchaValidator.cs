using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppCode.Extensions.GoogleRecaptchaV3
{
  /// <summary>
  /// Google reCAPTCHA v3 validator.
  /// Verifies token, score, and optional hostname.
  /// </summary>
  public class RecaptchaValidator : Custom.Hybrid.CodeTyped
  {
    private const string SiteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    /// <summary>
    /// Validate a reCAPTCHA v3 token against Google.
    /// </summary>
    /// <param name="token">Token received from client-side reCAPTCHA execution.</param>
    /// <param name="privateKey">Optional secret key override; otherwise read from settings.</param>
    /// <param name="remoteIp">Optional client IP (only send if you actually have it).</param>
    /// <param name="minimumScore">Score threshold (0..1). If negative, load from settings.</param>
    /// <param name="expectedHostname">Optional hostname to assert against the response.</param>
    public async Task<RecaptchaResult> ValidateAsync(
      string token,
      string privateKey = null,
      string remoteIp = null,
      double minimumScore = -1,
      string expectedHostname = null)
    {
      if (string.IsNullOrWhiteSpace(token))
        return RecaptchaResult.Err(RecaptchaErrors.MissingToken);

      var secret = !string.IsNullOrWhiteSpace(privateKey)
        ? privateKey
        : AllSettings.String("GoogleRecaptcha.PrivateKey", required: false);

      // If we still have no secret, fail immediately (matches Google error code)
      if (string.IsNullOrWhiteSpace(secret))
        return RecaptchaResult.Err(RecaptchaErrors.MissingSecret);

      // Minimum score fallback - if not set, retrieve, then ensure it's within bounds
      if (minimumScore < 0)
        minimumScore = AllSettings.Double("GoogleRecaptcha.ScoreThreshold", fallback: 0.5);

      // Normalize invalid thresholds to a safe default
      if (minimumScore < 0 || minimumScore > 1)
        minimumScore = 0.5;

      // Prepare form data for Google
      var form = new Dictionary<string, string>
      {
        { "secret", secret },
        { "response", token }
      };

      if (!string.IsNullOrWhiteSpace(remoteIp))
        form.Add("remoteip", remoteIp);

      // Ask Google to verify
      string body = null;
      try
      {
        // Create a HttpClient for the request
        using var httpClient = new HttpClient();
        var httpResponse = await httpClient
          .PostAsync(
            new Uri(SiteVerifyUrl),
            new FormUrlEncodedContent(form)
          )
          .ConfigureAwait(false);

        // Read raw JSON response
        body = await httpResponse.Content
          .ReadAsStringAsync()
          .ConfigureAwait(false);

        // empty body is not valid JSON
        if (string.IsNullOrWhiteSpace(body))
          return RecaptchaResult.Err(RecaptchaErrors.EmptyResponse);
      }
      catch (HttpRequestException)
      {
        // Network-level error (DNS, timeout, etc.)
        return RecaptchaResult.Err(RecaptchaErrors.NetworkError);
      }
      catch (Exception)
      {
        // Any other unexpected failure is also treated as network error
        return RecaptchaResult.Err(RecaptchaErrors.NetworkError);
      }

      RecaptchaResult google;
      
      try
      {
        // Parse JSON into our result model
        google = Kit.Json.To<RecaptchaResult>(body);
      }
      catch
      {
        // Invalid JSON or incompatible schema
        return RecaptchaResult.Err(RecaptchaErrors.InvalidResponse);
      }

      // If Google rejected, return their error codes
      if (!google.Success)
      {
        // Signal a 400 to the client to indicate failed validation
        Kit.Page.SetHttpStatus(400, RecaptchaErrors.CaptchaFailed);
        return google.ToError(string.Join(",", google.ErrorCodes));
      }

      // Optional hostname check to prevent token reuse on other domains
      if (!string.IsNullOrWhiteSpace(expectedHostname) && !string.Equals(google.Hostname, expectedHostname, StringComparison.OrdinalIgnoreCase))
        return google.ToError(RecaptchaErrors.ActionMismatch);

      // Enforce score threshold if Google provides a score
      if (google.Score.HasValue && minimumScore > 0 && google.Score.Value < minimumScore)
        return google.ToError(RecaptchaErrors.ScoreTooLow);

      // Success case: return the response model
      return google;
    }
  }
}

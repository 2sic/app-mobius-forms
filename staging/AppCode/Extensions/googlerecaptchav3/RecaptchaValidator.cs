using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppCode.Extensions.GoogleRecaptchaV3
{
  public class RecaptchaValidator: Custom.Hybrid.CodeTyped
  {
    private static readonly Uri SiteVerifyUri =
      new Uri("https://www.google.com/recaptcha/api/siteverify");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="privateKey">Optional Recaptcha Private Key - will auto-load from settings if not provided</param>
    /// <param name="remoteIp"></param>
    /// <param name="minimumScore"></param>
    /// <param name="expectedHostname"></param>
    /// <returns></returns>
    public async Task<RecaptchaResult> ValidateAsync(
      string token,
      string privateKey = null,
      string remoteIp = null,
      double minimumScore = -1,
      string expectedHostname = null
    )
    {
      if (string.IsNullOrWhiteSpace(token))
        return new RecaptchaResult { IsValid = false, Error = "token_missing" };

      if (string.IsNullOrWhiteSpace(privateKey))
      {
        privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value; // + "bb";
        if (string.IsNullOrWhiteSpace(privateKey))
          return new RecaptchaResult { IsValid = false, Error = "private_key_missing" };
      }

      if (minimumScore < 0 || minimumScore > 1)
      {
        minimumScore = AllSettings.Double("GoogleRecaptcha.ScoreThreshold");
        if (minimumScore < 0 || minimumScore > 1)
          return new RecaptchaResult { IsValid = false, Error = "invalid_minimum_score" };
      }

      using (var httpClient = new HttpClient())
      {
        var form = new Dictionary<string, string>
        {
          { "secret", privateKey },
          { "response", token }
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
          form.Add("remoteip", remoteIp);

        var response = await httpClient
          .PostAsync(SiteVerifyUri, new FormUrlEncodedContent(form))
          .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync()
          .ConfigureAwait(false);

        RecaptchaResponse captchaResponse;
        try
        {
          captchaResponse = Kit.Json.To<RecaptchaResponse>(body);
        }
        catch (Exception ex)
        {
          return new RecaptchaResult
          {
            IsValid = false,
            Error = "json_parse_failed",
            ErrorCodes = new[] { ex.Message }
          };
        }

        if (captchaResponse == null)
          return new RecaptchaResult { IsValid = false, Error = "empty_response" };

        var result = new RecaptchaResult
        {
          IsValid = captchaResponse.Success,
          Score = captchaResponse.Score,
          Hostname = captchaResponse.Hostname,
          ErrorCodes = captchaResponse.ErrorCodes
        };

        if (!captchaResponse.Success)
        {
           Kit.Page.SetHttpStatus(400, "captcha_failed");
          return result;
        }

        if (!string.IsNullOrWhiteSpace(expectedHostname) &&
            !string.Equals(captchaResponse.Hostname, expectedHostname, StringComparison.OrdinalIgnoreCase))
        {
          result.IsValid = false;
          result.Error = "hostname_mismatch";
          return result;
        }

        if (captchaResponse.Score.HasValue &&
            minimumScore > 0 &&
            captchaResponse.Score.Value < minimumScore)
        {
          result.IsValid = false;
          result.Error = "score_too_low";
          return result;
        }

        return result;
      }
    }
  }
}

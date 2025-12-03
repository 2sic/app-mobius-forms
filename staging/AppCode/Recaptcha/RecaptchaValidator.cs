using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCode.RecaptchaValidator
{
  public class Recaptcha : Custom.Hybrid.CodeTyped
  {
    private static readonly Uri SiteVerifyUri = new Uri("https://www.google.com/recaptcha/api/siteverify");

    public async Task<bool> Validate(string encodedResponse, string remoteIp = null, double minScore = 0.0)
    {
      var wrapLog = Log.Call();

      if (string.IsNullOrWhiteSpace(encodedResponse))
        throw new ArgumentException("recaptcha is empty", nameof(encodedResponse));

      var privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value;
      if (string.IsNullOrEmpty(privateKey))
      {
        Log.Add("recaptcha: private key missing");
        throw new InvalidOperationException("recaptcha private key not configured");
      }

      using (var client = new HttpClient())
      {
        // POST (secure) instead of GET
        var form = new List<KeyValuePair<string, string>>
        {
          new KeyValuePair<string, string>("secret", privateKey),
          new KeyValuePair<string, string>("response", encodedResponse)
        };
        if (!string.IsNullOrEmpty(remoteIp))
          form.Add(new KeyValuePair<string, string>("remoteip", remoteIp));

        var resp = await client.PostAsync(SiteVerifyUri, new FormUrlEncodedContent(form)).ConfigureAwait(false);
        var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

        RecaptchaResponse captchaResponse = null;
        try
        {
          captchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(body, new JsonSerializerOptions
          {
            PropertyNameCaseInsensitive = true
          });
        }
        catch (Exception ex)
        {
          Log.Add("recaptcha: json parse failed: " + ex.Message);
          throw;
        }

        if (captchaResponse == null)
        {
          Log.Add("recaptcha: empty response from google");
          return false;
        }

        // Always log key fields for monitoring (ohne das secret!)
        Log.Add($"recaptcha: success={captchaResponse.Success}, score={(captchaResponse.Score.HasValue ? captchaResponse.Score.Value.ToString("F2") : "n/a")}, hostname={captchaResponse.Hostname ?? "n/a"}, errors={(captchaResponse.ErrorCodes == null ? "none" : string.Join(",", captchaResponse.ErrorCodes))}");

        if (!captchaResponse.Success)
        {
          Log.Add("recaptcha check failed: success=false");
          return false;
        }

        // Hostname check - robust using Uri
        try
        {
          if (Uri.TryCreate(MyContext.Site.Url, UriKind.Absolute, out var siteUri))
          {
            if (!string.Equals(captchaResponse.Hostname, siteUri.Host, StringComparison.OrdinalIgnoreCase))
            {
              Log.Add($"recaptcha hostname mismatch: google='{captchaResponse.Hostname}' expected='{siteUri.Host}'");
              return false;
            }
          }
        }
        catch
        {
          // fallthrough: if MyContext.Site.Url malformed, don't block entirely but log
          Log.Add("recaptcha: could not parse MyContext.Site.Url for hostname comparison");
        }

        // If we have a v3 score: enforce threshold
        if (captchaResponse.Score.HasValue)
        {
          if (minScore > 0.0 && captchaResponse.Score.Value < minScore)
          {
            Log.Add($"recaptcha score too low: {captchaResponse.Score.Value:F2} < {minScore:F2}");
            return false;
          }
        }

        wrapLog("ok");
        return true;
      }
    }
  }
}
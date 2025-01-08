using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppCode.RecaptchaValidator
{
  // Helper to do Recaptcha server-validation
  // based on http://stackoverflow.com/questions/27764692/validating-recaptcha-2-no-captcha-recaptcha-in-asp-nets-server-side
  // shouldn't really need any modifications, just leave this as is
  public class Recaptcha : Custom.Hybrid.CodeTyped
  {
    public async Task<bool> Validate(string encodedResponse)
    {
      // Log what's happening in case we run into problems
      var wrapLog = Log.Call();

      if (!(encodedResponse is string) || String.IsNullOrEmpty(encodedResponse as string))
        throw new Exception("recaptcha is empty");

      // Get the private key from Settings - if it's from presets, it is encrypted
      var privateKey = Kit.SecureData.Parse(AllSettings.String("GoogleRecaptcha.PrivateKey")).Value;

      // Ask google if the verification is valid
      var client = new HttpClient();
      var GoogleReply = await client.GetStringAsync(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", privateKey, encodedResponse));
      var captchaResponse = Kit.Convert.Json.To<RecaptchaResponse>(GoogleReply);

      var status = captchaResponse.Success;
      var isSameSite = captchaResponse.Hostname == MyContext.Site.Url.Replace("https://", "").Split('/')[0];

      if (!status || !isSameSite)
      {
        Log.Add("recaptcha check failed:" + status);
        throw new Exception("bad recaptcha '" + status + "'");
      }

      wrapLog("ok");
      return status && isSameSite;
    }
  }
}




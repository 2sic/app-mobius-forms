using System.Collections.Generic;
using Newtonsoft.Json;

// Helper to to Recaptcha checks
// shouldn't really need any modifications, just leave this as is
public class RecaptchaHelper
{
  public bool Validate(string EncodedResponse, string PrivateKey)
  {
    var client = new System.Net.WebClient();
    var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
    var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<RecaptchaHelper>(GoogleReply);

    return captchaResponse.Success;// == "True";
  }

  [JsonProperty("success")]
  public bool Success
  {
    get { return m_Success; }
    set { m_Success = value; }
  }

  private bool m_Success;
  [JsonProperty("error-codes")]
  public List<string> ErrorCodes
  {
    get { return m_ErrorCodes; }
    set { m_ErrorCodes = value; }
  }


  private List<string> m_ErrorCodes;
}
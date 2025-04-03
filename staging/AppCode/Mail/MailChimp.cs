using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http; // is in use Line 86
using System.Linq;

using AppCode.Data;

namespace AppCode.MailChimp
{
  public class MailChimp : Custom.Hybrid.CodeTyped
  {
    /* MAILCHIMP SUBSCRIBE */
    public string Subscribe(Dictionary<string, object> formFields, string subscriberMailField, string mailchimpTagConfig)
    {
      // Log what's happening in case we run into problems
      Log.Add("MailChimp enabled - try to add");
      var formFieldsDict = formFields.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? string.Empty);

      var dicSource = Kit.Template.CreateSource("Form", formFieldsDict);
      var engine = Kit.Template.Empty(sources: new [] { dicSource });

      // Parse the template with the form fields to get optinal fields
      var fieldsTemplate = engine.Parse(mailchimpTagConfig);

      // Parse the template with the form fields to get the email address
      // This is the email address that will be used to subscribe the user to MailChimp
      var subscriberMail = engine.Parse(subscriberMailField);

      var wrapLog = Log.Call();
      var appSettings = As<AppSettings>(App.Settings);

      Log.Add("Email: " + subscriberMail);
      var msg = SubscribeToMailChimp(appSettings.MailchimpServer, appSettings.MailchimpListId, appSettings.MailchimpAPIKey, subscriberMail, fieldsTemplate);
      if (msg != "OK")
      {
        wrapLog("error");
        throw new Exception("Mailchimp registration failed - check EventLog - msg was " + msg);
      }
      wrapLog("ok");
      return "true";
    }

    private string SubscribeToMailChimp(string srv, string listId, string apiKey, string email, string fieldTemplate)
    {
      var baseUrl = "https://" + srv + ".api.mailchimp.com/3.0/lists/" + listId + "/members";

      var subscriberUrl = baseUrl + "/" + CreateMD5(email.ToLower());

      var mergeFields = fieldTemplate
            .Split('\n')
            .Select(line => line.Split('='))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1]);

      var body = new
      {
        email_address = email,
        status = "pending",
        merge_fields = mergeFields
      };

      // First check if user is already in list
      var response = MailchimpRequest(subscriberUrl, "GET", "", apiKey);
      if (response.StatusCode == HttpStatusCode.OK)
      {
        var typedJson = Kit.Json.ToTyped(response.Response);
        var currentStatus = typedJson.String("status");
        // Do nothing if user is already subscribed
        if (currentStatus == "subscribed") return "OK";

        // Update existing subscriber
        return MailchimpRequest(subscriberUrl, "PUT", Kit.Json.ToJson(body), apiKey).StatusCode.ToString();
      }
      return MailchimpRequest(baseUrl, "POST", Kit.Json.ToJson(body), apiKey).StatusCode.ToString();
    }

    private class MailchimpResponse
    {
      public string Response;
      public HttpStatusCode StatusCode;
    }

    private static readonly HttpClient client = new HttpClient() { Timeout = new TimeSpan(0, 0, 10) };

    /* HELPERS */
    /* CREATE MD5 OF INPUT */
    private static string CreateMD5(string input)
    {
      // Use input string to calculate MD5 hash
      using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        // Convert the byte array to hexadecimal string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
          sb.Append(hashBytes[i].ToString("X2"));
        }
        return sb.ToString();
      }
    }

    private MailchimpResponse MailchimpRequest(string url, string method, string body, string apiKey)
    {
      var logTimeStamp = DateTime.Now;
      Kit.SystemLog.Add("Mailchimp controller", logTimeStamp + " - will send " + method + " request to " + url + " with body " + body);

      String encodedApiKey = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("anystring" + ":" + apiKey));

      var httpMethod = new HttpMethod(method.ToUpper());
      var requestMessage = new HttpRequestMessage(httpMethod, url);
      requestMessage.Headers.Add("Authorization", "Basic " + encodedApiKey);

      if (method != "GET")
      {
        requestMessage.Content = new StringContent(body);
        requestMessage.Content.Headers.Remove("Content-Type");
        requestMessage.Content.Headers.Add("Content-Type", "application/json; charset=utf-8");
      }

      var responseMessage = client.SendAsync(requestMessage).Result;
      var response = responseMessage.Content.ReadAsStringAsync().Result;

      var r = new MailchimpResponse()
      {
        StatusCode = responseMessage.StatusCode,
        Response = response
      };
      Kit.SystemLog.Add("Mailchimp controller", logTimeStamp + " - got response: " + r.StatusCode + " with content " + r.Response);
      return r;
    }
  }
}


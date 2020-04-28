using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using DotNetNuke.Common;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;

// TODO: 2ro - check if we can just log to EAV and not to DNN
public class MailChimp : ToSic.Sxc.Dnn.DynamicCode
{
  // Checks for MailChimp Integration
  // if true instantiate mailchimp
  // subscribe for mailchimp
  public void Validate(Dictionary<string,object> contactFormRequest)
  {
    if(contactFormRequest.ContainsKey("MailChimp")) {
			Log.Add("MailChimp - see if we can add it...");
			if(contactFormRequest["MailChimp"].ToString() == "True") {
				Log.Add("...MailChimp - try to add");
				Subscribe(contactFormRequest);
			} else {
				Log.Add("...MailChimp - not wanted by user, won't add");
			}

			 
		} else {
			Log.Add("Won't add to MailChimp");
		}
  }
  /* MAILCHIMP SUBSCRIBE */
  public string Subscribe(Dictionary<string,object> contactFormRequest)
  {
    // Log what's happening in case we run into problems
    var wrapLog = Log.Call();

    var SenderName = (contactFormRequest.ContainsKey("SenderName") ? contactFormRequest["SenderName"].ToString() : "");
    var SenderLastName = (contactFormRequest.ContainsKey("SenderLastName") ? contactFormRequest["SenderLastName"].ToString() : "");
    Log.Add("Name:" + SenderName + ", LastName:" + SenderLastName);
    var msg = SubscribeToMailChimp(App.Settings.MailchimpServer, App.Settings.MailchimpListId, App.Settings.MailchimpAPIKey, contactFormRequest["SenderMail"].ToString(), SenderName, SenderLastName);
    if(msg != "OK")
    {
      wrapLog("error");
      throw new Exception("Mailchimp registration failed - check EventLog - msg was " + msg);
    }
    wrapLog("ok");
    return "true";
  }

  private string SubscribeToMailChimp(string srv, string listId, string apiKey, string email, string fname, string lname)
  {
    var baseUrl = "https://" + srv + ".api.mailchimp.com/3.0/lists/" + listId + "/members";

    var subscriberUrl = baseUrl + "/" + CreateMD5(email.ToLower());

    var body = new
    {
      email_address = email,
      status = "pending",
      merge_fields = new { FNAME = fname, LNAME = lname }
    };

    // First check if user is already in list
    var response = MailchimpRequest(subscriberUrl, "GET", "", apiKey);
    if(response.StatusCode == HttpStatusCode.OK)
    {
      var currentStatus = JsonConvert.DeserializeObject<dynamic>(response.Response).status;

      // Do nothing if user is already subscribed
      if (currentStatus == "subscribed") return "OK";
      
      // Update existing subscriber
      return MailchimpRequest(subscriberUrl, "PUT", JsonConvert.SerializeObject(body), apiKey).StatusCode.ToString();
    }
    else
    {
      return MailchimpRequest(baseUrl, "POST", JsonConvert.SerializeObject(body), apiKey).StatusCode.ToString();
    }
  }

  private class MailchimpResponse
  {
    public string Response;
    public HttpStatusCode StatusCode;
  }

  private static readonly HttpClient client = new HttpClient() { Timeout = new TimeSpan(0, 0, 10) };

  private MailchimpResponse MailchimpRequest(string url, string method, string body, string apiKey) {
    var logTimeStamp = DateTime.Now;
    EventLog("Mailchimp controller", logTimeStamp + " - will send " + method + " request to " + url + " with body " + body);
    
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
    EventLog("Mailchimp controller", logTimeStamp + " - got response: " + r.StatusCode + " with content " + r.Response);
    return r;
  }

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

	/* EVENTLOGGER */
	private void EventLog(string title, string message)
	{
    PortalSettings portalSettings = Globals.GetPortalSettings();
    var userInfo = UserController.Instance.GetCurrentUserInfo();
		var objEventLog = new EventLogController();
		objEventLog.AddLog(title, message, portalSettings, userInfo.UserID, EventLogController.EventLogType.ADMIN_ALERT);
	}
}
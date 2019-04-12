using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http;
using ToSic.SexyContent.WebApi;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using Newtonsoft.Json;
using System.IO;

public class FormController : SxcApiController
{
    
	[HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    [ValidateAntiForgeryToken]
    public void ProcessForm([FromBody]Dictionary<string,object> contactFormRequest)
    {

        // Pre-work: help the dictionary with the values uses case-insensitive key AccessLevel
        contactFormRequest = new Dictionary<string, object>(contactFormRequest, StringComparer.OrdinalIgnoreCase);

        // test exception to see how the js-side behaves on errors
        // throw new Exception();

        // 0. Pre-Check - validate recaptcha if used
        if(Content.Recaptcha ?? false) {
            var recap = contactFormRequest["Recaptcha"];
            if(!(recap is string) || String.IsNullOrEmpty(recap as string)) 
                throw new Exception("recaptcha is empty");
        
            // do server-validation
            // based on http://stackoverflow.com/questions/27764692/validating-recaptcha-2-no-captcha-recaptcha-in-asp-nets-server-side
            var ok = ReCaptchaClass.Validate(recap as string, App.Settings.RecaptchaSecretKey);
            if(!ok)
                throw new Exception("bad recaptcha '" + ok + "'" );
        }

        // after saving, remove recaptcha fields from the data-package,
        // because we don't want them in the e-mails
        removeKeys(contactFormRequest, new string[] { "g-recaptcha-response", "useRecaptcha",  "Recaptcha", "submit" }); 

        // get configuration for this Form
        // it is either customized at Content-level, or we should use the default in the App.Settings
        // the content-type is stored as the StaticName - but for the simple API we need the "nice name"
        var config = (Content.Presentation.SubmitType as IEnumerable<dynamic>).FirstOrDefault()
            ?? (App.Settings.SubmitType as IEnumerable<dynamic>).First();
        var type = Data.Cache.GetContentType(config.ContentType);

        // 1. add IP / host, and save all fields
        // if you add fields to your content-type, just make sure they are 
        // in the request with the correct name, they will be added automatically
        contactFormRequest.Add("Timestamp", DateTime.Now);
        contactFormRequest.Add("SenderIP", System.Web.HttpContext.Current.Request.UserHostAddress);
        contactFormRequest.Add("ModuleId", Dnn.Module.ModuleID);
        // add raw-data, in case the content-type has a "RawData" field
        contactFormRequest.Add("RawData", JsonConvert.SerializeObject(contactFormRequest));
        // add Title (if non given), in case the Content-Type would benefit of an automatic title
        var addTitle = !contactFormRequest.ContainsKey("Title");
        if(addTitle) contactFormRequest.Add("Title", "Form " + DateTime.Now.ToString("s"));
        // Add guid to identify entity after saving
        var guid = Guid.NewGuid();
        contactFormRequest.Add("EntityGuid", guid);
        App.Data.Create(type.Name, contactFormRequest);

        // 2018-09-18 added feature to create a full-save of each request into a system-protocol content-type
        App.Data.Create("SystemProtocol", contactFormRequest);

        var files = new List<ToSic.Sxc.Adam.IFile>();
        
        // Save files to Adam
        if(contactFormRequest.ContainsKey("Files")){
            foreach(var file in ((Newtonsoft.Json.Linq.JArray)contactFormRequest["Files"]).ToObject<IEnumerable<Dictionary<string, string>>>())
            {
                var data = Convert.FromBase64String((file["Encoded"]).Split(',')[1]);
                files.Add(SaveInAdam(stream: new MemoryStream(data), fileName: file["Name"], contentType: type.Name, guid: guid, field: file["Field"]));
            }

            // Don't keep Files array in ContactFormRequest
            removeKeys(contactFormRequest, new string[] { "Files" });
        }   

        // if(contactFormRequest.ContainsKey("MailChimp")){
        //     if(contactFormRequest['MailChimp']) {
        //         Subscribe(contactFormRequest['SenderMail'], contactFormRequest['SenderName'], contactFormRequest['SenderLastName']);
        //     }
        // }
        
        // 2. assemble all settings to send the mail
        // background: some settings are made in this module,
        // but if they are missing we use fallback settings 
        // which are taken from the App.Settings
		var settings = new {
            MailFrom = !String.IsNullOrEmpty(Content.MailFrom) ? Content.MailFrom : App.Settings.OwnerMail,
			OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : App.Settings.OwnerMail
		};
		

        // Pre 3: Improve keys / values for nicer presentation in the mail
        // after saving, remove raw-data and the generated title
        // because we don't want them in the e-mails
        removeKeys(contactFormRequest, new string[] { "RawData", addTitle ? "Title" : "some-fake-key" }); 

        // rewrite the keys to be a nicer format, based on the configuration
        string mailLabelRewrites = (!String.IsNullOrEmpty(config.MailLabels) 
            ? config.MailLabels
            : App.Settings.SubmitType[0].MailLabels) ?? "";
        var valuesWithMailLabels = RewriteKeys(contactFormRequest, mailLabelRewrites);

        // 3. Send Mail to owner
        // uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
        var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
        
        if(Content.OwnerSend != null && Content.OwnerSend){
            var ownerMailEngine = TemplateInstance(config.OwnerMailTemplate);
            var ownerBody = ownerMailEngine.Message(valuesWithMailLabels, this).ToString();
            var ownerSubj = ownerMailEngine.Subject(valuesWithMailLabels, this);

            var attachments = files.Select(f =>
                new System.Net.Mail.Attachment(
                    new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + f.Url, FileMode.Open), f.FullName)).ToList();

            Mail.SendMail(settings.MailFrom, settings.OwnerMail, Content.OwnerMailCC, "", custMail, MailPriority.Normal,
                ownerSubj, MailFormat.Html, System.Text.Encoding.UTF8, ownerBody, attachments, "", "", "", "", false);
        }

        // 4. Send Mail to customer
        if(Content.CustomerSend != null && Content.CustomerSend && !String.IsNullOrEmpty(custMail)){
            var customerMailEngine = TemplateInstance(config.CustomerMailTemplate);
            var customerBody = customerMailEngine.Message(valuesWithMailLabels, this).ToString();
            var customerSubj = customerMailEngine.Subject(valuesWithMailLabels, this);

            Mail.SendMail(settings.MailFrom, custMail, Content.CustomerMailCC, "", settings.OwnerMail, MailPriority.Normal,
                customerSubj, MailFormat.Html, System.Text.Encoding.UTF8, customerBody, new string[0], "", "", "", "", false);
        }
    }

    private dynamic TemplateInstance(string fileName)
    {
        var compiledType = BuildManager.GetCompiledType(System.IO.Path.Combine("~", App.Path, "email-templates", fileName));
        object objectValue = null;
        if (compiledType != null)
        {
            objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            return ((dynamic)objectValue);
        }
        throw new Exception("Error while creating mail template instance.");
    }


    private void removeKeys(Dictionary<string,object> contactFormRequest, string[] badKeys)
    {
        foreach (var key in badKeys)
            if(contactFormRequest.ContainsKey(key)) 
                contactFormRequest.Remove(key);
    }

    private Dictionary<string, object> RewriteKeys(Dictionary<string, object> dic, string map)
    {
        // create keys-map
        Dictionary<string, string> newKeys = map
            .Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
            .ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1], StringComparer.OrdinalIgnoreCase);

        return dic.ToDictionary(g => newKeys.ContainsKey(g.Key) ? newKeys[g.Key] : g.Key, g => g.Value, StringComparer.OrdinalIgnoreCase);
    }

    private string Subscribe(string email, string fname, string lname)
    {
        var msg = SubscribeToMailChimp(App.Settings.MailchimpServer, App.Settings.MailchimpListId, App.Settings.MailchimpAPIKey, email, fname, lname);
        if(msg != "OK")
        {
            throw new Exception("Mailchimp registration failed - check EventLog - msg was " + msg);
        }
        return "test";
    }

    private string SubscribeToMailChimp(string srv, string listId, string apiKey, string email, string fname, string lname)
    {
        var baseUrl = "https://" + srv + ".api.mailchimp.com/3.0/lists/" + listId + "/members";

        var subscriberUrl = baseUrl + "/" + CreateMD5(email.ToLower());
        return "test2";

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

    private void EventLog(string title, string message)
    {
        var objEventLog = new EventLogController();
        objEventLog.AddLog(title, message, PortalSettings, this.UserInfo.UserID, EventLogController.EventLogType.ADMIN_ALERT);
    }
    
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
}


// Helper to to Recaptcha checks
// shouldn't really need any modifications, just leave this as is
public class ReCaptchaClass
{
    public static bool Validate(string EncodedResponse, string PrivateKey)
    {
        var client = new System.Net.WebClient();
        var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
        var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);

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
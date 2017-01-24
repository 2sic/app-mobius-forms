using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http;
using ToSic.SexyContent.WebApi;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Mail;
using Newtonsoft.Json;

public class FormController : SxcApiController
{
    
	[HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    [ValidateAntiForgeryToken]
    public void ProcessForm([FromBody]Dictionary<string,object> contactFormRequest)
    {
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
        if(!contactFormRequest.ContainsKey("Title"))
            contactFormRequest.Add("Title", "Form " + DateTime.Now.ToString("s"));
        App.Data.Create(type.Name, contactFormRequest);

        // after saving, remove recaptcha fields from the data-package,
        // because we don't want them in the e-mails
        var badKeys = new string[] { "g-recaptcha-response", "useRecaptcha",  "Recaptcha", "submit"}; 
        foreach (var key in badKeys)
            if(contactFormRequest.ContainsKey(key)) 
                contactFormRequest.Remove(key);

        // 2. assemble all settings to send the mail
        // background: some settings are made in this module,
        // but if they are missing we use fallback settings 
        // which are taken from the App.Settings
		var settings = new {
            MailFrom = !String.IsNullOrEmpty(Content.MailFrom) ? Content.MailFrom : App.Settings.OwnerMail,
			OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : App.Settings.OwnerMail
		};
		

        // 3. Send Mail to owner
        // uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
        var ownerMailEngine = TemplateInstance(config.OwnerMailTemplate);
        var ownerBody = ownerMailEngine.Message(contactFormRequest, this).ToString();
        var ownerSubj = ownerMailEngine.Subject(contactFormRequest, this);
        var custMail = contactFormRequest["SenderMail"].ToString();

        Mail.SendMail(settings.MailFrom, settings.OwnerMail, Content.OwnerMailCC, "", custMail, MailPriority.Normal,
            ownerSubj, MailFormat.Html, System.Text.Encoding.UTF8, ownerBody, new string[0], "", "", "", "", false);

        // 4. Send Mail to customer
        var customerMailEngine = TemplateInstance(config.CustomerMailTemplate);
        var customerBody = customerMailEngine.Message(contactFormRequest, this).ToString();
        var customerSubj = customerMailEngine.Subject(contactFormRequest, this);

        Mail.SendMail(settings.MailFrom, custMail, Content.CustomerMailCC, "", settings.OwnerMail, MailPriority.Normal,
            customerSubj, MailFormat.Html, System.Text.Encoding.UTF8, customerBody, new string[0], "", "", "", "", false);
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
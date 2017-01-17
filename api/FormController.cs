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

        // 1. add IP / host, and save all fields
        // if you add fields to your content-type, just make sure they are 
        // in the request with the correct name, they will be added automatically
        var typeName = String.IsNullOrEmpty(Content.ContentType) ? App.Settings.DefaultContentType : Content.ContentType;
        contactFormRequest.Add("Timestamp", DateTime.Now);
        contactFormRequest.Add("SenderIP", System.Web.HttpContext.Current.Request.UserHostAddress);
        contactFormRequest.Add("ModuleId", Dnn.Module.ModuleID);
        App.Data.Create(typeName, contactFormRequest);


        // 2. assemble all settings to send the mail
        // background: some settings are made in this module,
        // but if they are missing we use fallback settings 
        // which are taken from the App.Settings
		var settings = new {
			MailFrom = Content.MailFrom,
			OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : App.Settings.OwnerMail,
			OwnerMailCC = Content.OwnerMailCC,
			OwnerMailTemplateFile = !String.IsNullOrEmpty(Content.OwnerMailTemplateFile) ? Content.OwnerMailTemplateFile : App.Settings.OwnerMailTemplateFile,
			CustomerMailCC = Content.CustomerMailCC,
			CustomerMailTemplateFile = !String.IsNullOrEmpty(Content.CustomerMailTemplateFile) ? Content.CustomerMailTemplateFile : App.Settings.CustomerMailTemplateFile
		};
		

        // 3. Send Mail to owner
        var ownerMailFile = TemplateInstance(settings.OwnerMailTemplateFile);
        var ownerMail = ownerMailFile.Message(contactFormRequest, this).ToString();

        Mail.SendMail(settings.MailFrom, settings.OwnerMail, settings.OwnerMailCC, "", MailPriority.Normal,
            ownerMailFile.Subject(contactFormRequest, this), MailFormat.Html, System.Text.Encoding.UTF8, ownerMail, "", "", "", "", "");

        // 4. Send Mail to customer
        var customerMailFile = TemplateInstance(settings.CustomerMailTemplateFile);
        var customerMail = customerMailFile.Message(contactFormRequest, this).ToString();

        Mail.SendMail(settings.MailFrom, contactFormRequest["SenderMail"].ToString(), settings.CustomerMailCC, "", MailPriority.Normal,
            customerMailFile.Subject(contactFormRequest, this), MailFormat.Html, System.Text.Encoding.UTF8, customerMail, "", "", "", "", "");
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
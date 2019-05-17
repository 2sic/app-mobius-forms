using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

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
			var gRecap = ReCaptchaInstance();
			var ok = gRecap.Validate(recap as string, App.Settings.RecaptchaSecretKey);
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
		

		
		if(contactFormRequest.ContainsKey("MailChimp")){
			var mChimp = MailChimpInstance();
			mChimp.Subscribe(PortalSettings, App.Settings.MailchimpServer, App.Settings.MailchimpListId, App.Settings.MailchimpAPIKey, contactFormRequest["SenderMail"].ToString(), contactFormRequest["SenderName"].ToString(), contactFormRequest["SenderLastName"].ToString());
		}
		
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
			removeKeys(contactFormRequest, new string[] { "EntityGuid", "ModuleId",  "SenderIP", "Timestamp", "sendFormWithApi" }); 
			
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
			removeKeys(contactFormRequest, new string[] { "EntityGuid", "ModuleId",  "SenderIP", "Timestamp", "sendFormWithApi" }); 

			var customerMailEngine = TemplateInstance(config.CustomerMailTemplate);
			var customerBody = customerMailEngine.Message(valuesWithMailLabels, this).ToString();
			var customerSubj = customerMailEngine.Subject(valuesWithMailLabels, this);

			Mail.SendMail(settings.MailFrom, custMail, Content.CustomerMailCC, "", settings.OwnerMail, MailPriority.Normal,
				customerSubj, MailFormat.Html, System.Text.Encoding.UTF8, customerBody, new string[0], "", "", "", "", false);
		}
	}

	/* HELPERS */
	/* EVENTLOGGER */
	private void EventLog(string title, string message)
	{
		var objEventLog = new EventLogController();
		objEventLog.AddLog(title, message, PortalSettings, this.UserInfo.UserID, EventLogController.EventLogType.ADMIN_ALERT);
	}

	/* REMOVE KEY FROM HEADER */
	private void removeKeys(Dictionary<string,object> contactFormRequest, string[] badKeys)
	{
		foreach (var key in badKeys)
			if(contactFormRequest.ContainsKey(key)) 
				contactFormRequest.Remove(key);
	}

	/* CREATES A DICTIONARY */
	private Dictionary<string, object> RewriteKeys(Dictionary<string, object> dic, string map)
	{
		// create keys-map
		Dictionary<string, string> newKeys = map
			.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
			.ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1], StringComparer.OrdinalIgnoreCase);

		return dic.ToDictionary(g => newKeys.ContainsKey(g.Key) ? newKeys[g.Key] : g.Key, g => g.Value, StringComparer.OrdinalIgnoreCase);
	}

	/* GET EMAIL TEMPLATE */
	private dynamic TemplateInstance(string fileName)
	{
		var compiledType = BuildManager.GetCompiledType(System.IO.Path.Combine("~", App.Path, "staging/email-templates", fileName));
		object objectValue = null;
		if (compiledType != null)
		{
			objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
			return ((dynamic)objectValue);
		}
		throw new Exception("Error while creating mail template instance.");
	}

	/* INSTANTIATE RECAPTCHA CLASS */
	private dynamic ReCaptchaInstance()
	{
		var path = System.IO.Path.Combine("~", App.Path, "staging/api", "RecaptchaHelper.cs");
		//var compiledType = BuildManager.GetCompiledType(path);

		var assembly = BuildManager.GetCompiledAssembly(path);
    var compiledType = assembly.GetType("RecaptchaHelper", true, true);

		object objectValue = null;
		if (compiledType != null)
		{
			objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
			return ((dynamic)objectValue);
		}
		return objectValue;
	}

	/* INSTANTIATE MAILCHIMP CLASS */
	private dynamic MailChimpInstance()
	{
		var path = System.IO.Path.Combine("~", App.Path, "staging/api", "MailChimpHelper.cs");
		//var compiledType = BuildManager.GetCompiledType(path);

		var assembly = BuildManager.GetCompiledAssembly(path);
    var compiledType = assembly.GetType("MailChimpHelper", true, true);

		object objectValue = null;
		if (compiledType != null)
		{
			objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
			return ((dynamic)objectValue);
		}
		return objectValue;
	}
}
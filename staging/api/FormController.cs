using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;
using Newtonsoft.Json;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;

public class FormController : ToSic.Sxc.Dnn.ApiController
{
	[HttpPost]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	[ValidateAntiForgeryToken]
	public void ProcessForm([FromBody]Dictionary<string,object> contactFormRequest)
	{
		var wrapLog = Log.Call(useTimer: true);
		// Pre-work: help the dictionary with the values uses case-insensitive key AccessLevel
		contactFormRequest = new Dictionary<string, object>(contactFormRequest, StringComparer.OrdinalIgnoreCase);
    var config = Content; // the Content-Object is auto-prefilled with the configuration of the form


		// 0. Pre-Check - validate recaptcha if enabled in the Content object (the form configuration)
		if(config.Recaptcha ?? false) {
			Log.Add("checking Recaptcha");
			CreateInstance("Parts/Recaptcha.cs").Validate(contactFormRequest["Recaptcha"] as string, App.Settings.RecaptchaSecretKey, this);
		}

		// 0.1. after saving, remove recaptcha fields from the data-package, because we don't want them in the e-mails
		removeKeys(contactFormRequest, new string[] { "g-recaptcha-response", "useRecaptcha",  "Recaptcha", "submit" }); 

		// get configuration for this Form
		// it is either customized at Content-level, or we should use the default in the App.Settings
		// the content-type is stored as the StaticName - but for the simple API we need the "nice name"
		var saveSendConfig = (config.Presentation.SubmitType as Dynlist).FirstOrDefault() ?? (App.Settings.SubmitType as Dynlist).First();

		// 1. add IP / host, and save all fields
		// if you add fields to your content-type, just make sure they are 
		// in the request with the correct name, they will be added automatically
		contactFormRequest.Add("Timestamp", DateTime.Now);
		contactFormRequest.Add("SenderIP", System.Web.HttpContext.Current.Request.UserHostAddress);
		contactFormRequest.Add("ModuleId", Dnn.Module.ModuleID);
		// add raw-data, in case the content-type has a "RawData" field
		contactFormRequest.Add("RawData", createRawDataEntry(contactFormRequest));
		// add Title (if non given), in case the Content-Type would benefit of an automatic title
		var addTitle = !contactFormRequest.ContainsKey("Title");
		if(addTitle) contactFormRequest.Add("Title", "Form " + DateTime.Now.ToString("s"));

		// Automatically full-save each request into a system-protocol content-type
		// This helps to debug or find submissions in case something wasn't configured right
		Log.Add("Save data to SystemProtocol in case we ever need to see what was submitted");
		App.Data.Create("SystemProtocol", contactFormRequest);

		// Add guid to identify entity after saving (because we need to find it afterwards)
		var guid = Guid.NewGuid();
		contactFormRequest.Add("EntityGuid", guid);
		Log.Add("Save data to content type");
		App.Data.Create(saveSendConfig.ContentType, contactFormRequest);

		var files = new List<ToSic.Sxc.Adam.IFile>();

		// Save files to Adam
		if(contactFormRequest.ContainsKey("Files")) {
			Log.Add("Found files, will save");
			foreach(var file in ((Newtonsoft.Json.Linq.JArray)contactFormRequest["Files"]).ToObject<IEnumerable<Dictionary<string, string>>>())
			{
				var data = Convert.FromBase64String((file["Encoded"]).Split(',')[1]);
				files.Add(SaveInAdam(stream: new MemoryStream(data), fileName: file["Name"], contentType: saveSendConfig.ContentType, guid: guid, field: file["Field"]));
			}

			// Don't keep Files array in ContactFormRequest
			removeKeys(contactFormRequest, new string[] { "Files" });
		} else {
			Log.Add("No files found to save");
		}

		CreateInstance("Parts/MailChimp.cs").Validate(contactFormRequest);
		// after subscribe, remove mailchimp field from the data-package,
		// because we don't want them in the e-mails
		removeKeys(contactFormRequest, new string[] { "MailChimp" });

		// Improve keys / values for nicer presentation in the mail
		// after saving, remove raw-data and the generated title
		// because we don't want them in the e-mails
		removeKeys(contactFormRequest, new string[] { "RawData", addTitle ? "Title" : "some-fake-key" }); 
		
		// remove App informations from data-package
		removeKeys(contactFormRequest, new string[] { "EntityGuid", "ModuleId",  "SenderIP", "Timestamp" }); 

		// sending Mails
		var sendMail = CreateInstance("Parts/SendMail.cs");
		sendMail.sendMails(contactFormRequest, files);
		
		wrapLog("ok");
	}

	private dynamic createRawDataEntry(Dictionary<string,object> formRequest)
	{
		var data = new Dictionary<string, object>(formRequest, StringComparer.OrdinalIgnoreCase);
		data.Remove("Files");
		return JsonConvert.SerializeObject(data);
	}

	// helpers
	// remove key from header
	private void removeKeys(Dictionary<string,object> contactFormRequest, string[] badKeys)
	{
		foreach (var key in badKeys)
			if(contactFormRequest.ContainsKey(key)) 
				contactFormRequest.Remove(key);
	}
}
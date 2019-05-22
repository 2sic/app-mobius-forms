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

public class FormController : SxcApiController
{
	[HttpPost]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
	[ValidateAntiForgeryToken]
	public void ProcessForm([FromBody]Dictionary<string,object> contactFormRequest)
	{
		// Pre-work: help the dictionary with the values uses case-insensitive key AccessLevel
		contactFormRequest = new Dictionary<string, object>(contactFormRequest, StringComparer.OrdinalIgnoreCase);

		// test exception for development to see how the js-side behaves on errors
		// throw new Exception();

		// 0. Pre-Check - validate recaptcha if enabled in the Content object (the form configuration)
		if(Content.Recaptcha ?? false) {
			InstantiateClass("Recaptcha")
				.Validate(contactFormRequest["Recaptcha"] as string, App.Settings.RecaptchaSecretKey);
		}

		// after saving, remove recaptcha fields from the data-package,
		// because we don't want them in the e-mails
		removeKeys(contactFormRequest, new string[] { "g-recaptcha-response", "useRecaptcha",  "Recaptcha", "submit" }); 

		// get configuration for this Form
		// it is either customized at Content-level, or we should use the default in the App.Settings
		// the content-type is stored as the StaticName - but for the simple API we need the "nice name"
		var config = (Content.Presentation.SubmitType as Dynlist).FirstOrDefault() ?? (App.Settings.SubmitType as Dynlist).First();
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
		// Add guid to identify entity after saving (because we need to find it afterwards)
		var guid = Guid.NewGuid();
		contactFormRequest.Add("EntityGuid", guid);
		App.Data.Create(type.Name, contactFormRequest);

		// Automatically full-save each request into a system-protocol content-type
		// This helps to debug or find submissions in case something wasn't configured right
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

		// Checks for MailChimp Integration
		// if true instantiate mailchimp
		// subscribe for mailchimp
		if(contactFormRequest.ContainsKey("MailChimp")) {
			if(contactFormRequest["MailChimp"].ToString() == "True") {
				InstantiateClass("MailChimp").Subscribe(App, contactFormRequest);
			}
			// after subscribe, remove mailchimp field from the data-package,
			// because we don't want them in the e-mails
			removeKeys(contactFormRequest, new string[] { "MailChimp" }); 
		}

		// Improve keys / values for nicer presentation in the mail
		// after saving, remove raw-data and the generated title
		// because we don't want them in the e-mails
		removeKeys(contactFormRequest, new string[] { "RawData", addTitle ? "Title" : "some-fake-key" }); 

		var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
		
		// remove App informations from data-package
		removeKeys(contactFormRequest, new string[] { "EntityGuid", "ModuleId",  "SenderIP", "Timestamp" }); 

		// assemble all settings to send the mail
		// background: some settings are made in this module,
		// but if they are missing we use fallback settings 
		// which are taken from the App.Settings
		var settings = new {
			MailFrom = !String.IsNullOrEmpty(Content.MailFrom) ? Content.MailFrom : App.Settings.OwnerMail,
			OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : App.Settings.OwnerMail
		};

		// rewrite the keys to be a nicer format, based on the configuration
		string mailLabelRewrites = (!String.IsNullOrEmpty(config.MailLabels) 
			? config.MailLabels
			: App.Settings.SubmitType[0].MailLabels) ?? "";
		var valuesWithMailLabels = RewriteKeys(contactFormRequest, mailLabelRewrites);

		// Send Mail to owner
		if(Content.OwnerSend != null && Content.OwnerSend) {
			InstantiateClass("SendMail").send(
				config, config.OwnerMailTemplate, valuesWithMailLabels, settings.MailFrom, settings.OwnerMail, Content.OwnerMailCC, custMail, files, App.Path,	this
			);
		}

		// Send Mail to customer
		if(Content.CustomerSend != null && Content.CustomerSend && !String.IsNullOrEmpty(custMail)) {
			InstantiateClass("SendMail").send(
				config, config.CustomerMailTemplate, valuesWithMailLabels, settings.MailFrom, custMail, Content.CustomerMailCC, settings.OwnerMail, files, App.Path, this
			);
		}
	}

	// helpers
	// remove key from header
	private void removeKeys(Dictionary<string,object> contactFormRequest, string[] badKeys)
	{
		foreach (var key in badKeys)
			if(contactFormRequest.ContainsKey(key)) 
				contactFormRequest.Remove(key);
	}

	// Get current edition (live/staging)
	private string getEdition(){
		var path = HttpContext.Current.Request.Url.AbsolutePath;
		return path.IndexOf("staging") > 0 ? "staging" : "live";
	}

  // rewrite the keys to be a nicer format, based on the configuration
	private Dictionary<string, object> RewriteKeys(Dictionary<string, object> dic, string map)
	{
		// create keys-map
		Dictionary<string, string> newKeys = map
			.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
			.ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1], StringComparer.OrdinalIgnoreCase);

		return dic.ToDictionary(g => newKeys.ContainsKey(g.Key) ? newKeys[g.Key] : g.Key, g => g.Value, StringComparer.OrdinalIgnoreCase);
	}

	// instantiate class
	private dynamic InstantiateClass(string name){
		var fileName = name + ".cs";
		var path = System.IO.Path.Combine("~", App.Path, getEdition() , "api/Parts", fileName);
		var assembly = BuildManager.GetCompiledAssembly(path);
    var compiledType = assembly.GetType(name, true, true);

		object objectValue = null;
		if (compiledType != null)
		{
			objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
			return ((dynamic)objectValue);
		}
		throw new Exception("Error while creating class instance.");
	}
}
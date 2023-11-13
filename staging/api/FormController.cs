// Add namespaces to enable security in Oqtane & Dnn despite the differences
#if NETCOREAPP
  using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
  using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Sxc.WebApi;

// WIP
using System.Text.Json;
using System.Text.Json.Nodes;

[AllowAnonymous]	// define that all commands can be accessed without a login
[JsonFormatter]   // Use modern JSON formatter
public class FormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  public void ProcessForm([FromBody] Dictionary<string, object> contactFormRequest, string workflowId)
  {
    var wrapLog = Log.Call(useTimer: true);
    // Pre-work: help the dictionary with the values uses case-insensitive key AccessLevel
    contactFormRequest = new Dictionary<string, object>(contactFormRequest, StringComparer.OrdinalIgnoreCase);

    // 0. Pre-Check - validate recaptcha if enabled in the MyContent object (the form configuration)
    var formConfig = MyItem;
    if (formConfig.Bool("Recaptcha"))
    {
      Log.Add("checking Recaptcha");
      GetCode("Parts/Recaptcha.cs").Validate(contactFormRequest["Recaptcha"] as string);
    }

    // 0.1. after saving, remove recaptcha fields from the data-package, because we don't want them in the e-mails
    RemoveKeys(contactFormRequest, new string[] { "g-recaptcha-response", "useRecaptcha", "Recaptcha", "submit" });

    // get configuration for this Form
    var workflow = AsItems(App.Data["Workflow"]).Where(w => w.String("WorkflowId") == workflowId).FirstOrDefault();

    // 1. add IP / host, and save all fields
    // if you add fields to your content-type, just make sure they are
    // in the request with the correct name, they will be added automatically
    contactFormRequest["Timestamp"] = DateTime.Now;
    // Add the SenderIP in case we need to track down abuse
#if NETCOREAPP
      contactFormRequest["SenderIP"] = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
#else
    contactFormRequest["SenderIP"] = System.Web.HttpContext.Current.Request.UserHostAddress;
#endif
    // Add the ModuleId to assign each sent form to a specific module
    contactFormRequest["ModuleId"] = MyContext.Module.Id;
    // add raw-data, in case the content-type has a "RawData" field
    contactFormRequest["RawData"] = CreateRawDataEntry(contactFormRequest);
    // add Title (if non given), in case the content-type would benefit of an automatic title
    var addTitle = !contactFormRequest.ContainsKey("Title");
    if (addTitle) contactFormRequest["Title"] = "Form " + DateTime.Now.ToString("s");

    // Automatically full-save each request into a system-protocol content-type
    // This helps to debug or find submissions in case something wasn't configured right
    Log.Add("Save data to SystemProtocol in case we ever need to see what was submitted");
    App.Data.Create("SystemProtocol", contactFormRequest);

    // Add guid to identify entity after saving (because we need to find it afterwards)
    var guid = Guid.NewGuid();
    contactFormRequest["EntityGuid"] = guid;
    Log.Add("Save data to content type");
    var dataTypeToCreate = workflow.String("ContentType");
    App.Data.Create(dataTypeToCreate, contactFormRequest);

    // Remove Terms and GDPR from the data-package - we don't want them in the e-mails
    RemoveKeys(contactFormRequest, new string[] { "GDPR", "Terms" });

    var files = new List<ToSic.Sxc.Adam.IFile>();

    // Save files to Adam
    if (contactFormRequest.ContainsKey("Files"))
    {
      foreach (var fileObj in contactFormRequest["Files"] as IEnumerable<object>)
      {
        var json = fileObj.ToString();
        var uploaded = Kit.Json.To<FileUpload>(json);
       
       var data = System.Convert.FromBase64String(uploaded.Encoded.Split(',')[1]);
        files.Add(SaveInAdam(stream: new MemoryStream(data), fileName: uploaded.Name, contentType: dataTypeToCreate, guid: guid, field: uploaded.Field));
      }
      // Don't keep Files array in ContactFormRequest
      RemoveKeys(contactFormRequest, new string[] { "Files" });
    }
    else
    {
      Log.Add("No files found to save");
    }

    // return "test";

    GetCode("Parts/MailChimp.cs").SubscribeIfEnabled(contactFormRequest);
    // after subscribe, remove mailchimp fields from the data-package because we don't want them in the e-mails
    RemoveKeys(contactFormRequest, new string[] { "MailChimp" });

    // Improve keys / values for nicer presentation in the mail
    // after saving, remove raw-data and the generated title
    // because we don't want them in the e-mails
    RemoveKeys(contactFormRequest, new string[] { "RawData", addTitle ? "Title" : "some-fake-key" });

    // remove App informations from data-package
    RemoveKeys(contactFormRequest, new string[] { "EntityGuid", "ModuleId", "SenderIP", "Timestamp" });

    // sending Mails
    var sendMail = GetCode("Parts/SendMail.cs");
    sendMail.SendMails(contactFormRequest, workflowId, files);

    wrapLog("ok");
  }

  private object CreateRawDataEntry(Dictionary<string, object> formRequest)
  {
    var data = new Dictionary<string, object>(formRequest, StringComparer.OrdinalIgnoreCase);
    data.Remove("Files");
    return Kit.Json.ToJson(data);
  }

  // helpers
  private void RemoveKeys(Dictionary<string, object> contactFormRequest, string[] badKeys)
  {
    foreach (var key in badKeys)
      if (contactFormRequest.ContainsKey(key))
        contactFormRequest.Remove(key);
  }
}

// @2dg - for now
public class FileUpload
{
  public string Field { get; set; }
  public string Name { get; set; }
  public string Encoded { get; set; }

}

// @2dg - for improvements afterwards
public class FuturePostWip
{
  public Dictionary<string, string> Fields { get; set; }
  public List<FileUpload> Files { get; set; }
  public Dictionary<string, string> Recaptcha { get; set; }
}

// 2sxclint:disable:no-dnn-namespaces
// 2sxclint:disable:no-web-namespaces
// 2sxclint:disable:no-EntityGuid-in-quotes

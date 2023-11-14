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
[AllowAnonymous]	// define that all commands can be accessed without a login
public class FormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  public void ProcessForm([FromBody] SaveRequest contactFormRequest, string workflowId)
  {

    // Copy the data into a new variable, as only this will be sent per Mail and the Other Data is need to Save in the 2sxc
    var fieldsFormRequest = new Dictionary<string, object>(contactFormRequest.Fields, StringComparer.OrdinalIgnoreCase);

    var wrapLog = Log.Call(useTimer: true);

    // 0. Pre-Check - validate recaptcha if enabled in the MyContent object (the form configuration)
    var formConfig = MyItem;
    if (formConfig.Bool("Recaptcha"))
    {
      Log.Add("checking Recaptcha");
      GetCode("Parts/Recaptcha.cs").Validate(contactFormRequest.Recaptcha);
    }

    // get configuration for this Form
    var workflow = AsItems(App.Data["Workflow"]).Where(w => w.String("WorkflowId") == workflowId).FirstOrDefault();

    // 1. add IP / host, and save all fields
    // if you add fields to your content-type, just make sure they are
    // in the request with the correct name, they will be added automatically
    contactFormRequest.Fields.Add("Timestamp", DateTime.Now);
    // Add the SenderIP in case we need to track down abuse
#if NETCOREAPP
    contactFormRequest.Fields.Add("SenderIP", Request.HttpContext.Connection.RemoteIpAddress?.ToString());
#else
    contactFormRequest.Fields.Add("SenderIP", System.Web.HttpContext.Current.Request.UserHostAddress);
#endif
    // Add the ModuleId to assign each sent form to a specific module
    contactFormRequest.Fields.Add("ModuleId", MyContext.Module.Id);
    // add raw-data, in case the content-type has a "RawData" field
    contactFormRequest.Fields.Add("RawData", CreateRawDataEntry(contactFormRequest));

    // add Title (if non given), in case the content-type would benefit of an automatic title
    var addTitle = !contactFormRequest.Fields.ContainsKey("Title");
    if (addTitle) contactFormRequest.Fields.Add("Title", "Form " + DateTime.Now.ToString("s"));
    // Automatically full-save each request into a system-protocol content-type
    // This helps to debug or find submissions in case something wasn't configured right
    Log.Add("Save data to SystemProtocol in case we ever need to see what was submitted");
    App.Data.Create("SystemProtocol", contactFormRequest.Fields);

    // Add guid to identify entity after saving (because we need to find it afterwards)
    var guid = Guid.NewGuid();
    contactFormRequest.Fields.Add("EntityGuid", guid);
    Log.Add("Save data to content type");
    var dataTypeToCreate = workflow.String("ContentType");
    App.Data.Create(dataTypeToCreate, contactFormRequest.Fields);

    var files = new List<ToSic.Sxc.Adam.IFile>();

    // Save files to Adam
    if (contactFormRequest.Files != null)
    {
      foreach (var fileObj in contactFormRequest.Files)
      {
        var data = System.Convert.FromBase64String(fileObj.Encoded.Split(',')[1]);
        files.Add(SaveInAdam(stream: new MemoryStream(data), fileName: fileObj.Name, contentType: dataTypeToCreate, guid: guid, field: fileObj.Field));
      }
    }
    else
    {
      Log.Add("No files found to save");
    }

    GetCode("Parts/MailChimp.cs").SubscribeIfEnabled(contactFormRequest.Fields);

    // sending Mails
    var sendMail = GetCode("Parts/SendMail.cs");
    // fieldsFormRequest are only the Form Data 
    sendMail.SendMails(fieldsFormRequest, workflowId, files);

    wrapLog("ok");
  }

  private object CreateRawDataEntry(SaveRequest formRequest)
  {
    var data = new Dictionary<string, object>();
    data.Add("Fields", formRequest.Fields);
    data.Add("Terms", formRequest.Terms);
    return Kit.Json.ToJson(data);
  }

}
public class FileUpload
{
  public string Field { get; set; }
  public string Name { get; set; }
  public string Encoded { get; set; }
  public byte[] Contents { get { return System.Convert.FromBase64String(Encoded.Split(',')[1]); } }
}

public class SaveRequest
{
  public Dictionary<string, object> Fields { get; set; }
  public List<FileUpload> Files { get; set; }
  public Dictionary<string, string> Terms { get; set; }
  public string Recaptcha { get; set; }
  public bool MailChimp { get; set; }
}

// 2sxclint:disable:no-dnn-namespaces
// 2sxclint:disable:no-web-namespaces
// 2sxclint:disable:no-EntityGuid-in-quotes

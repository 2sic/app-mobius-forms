// Add namespaces to enable security in Oqtane & Dnn despite the differences
#if NETCOREAPP
  using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
  using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;
#endif
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using AppCode.Data;
using AppCode.MailChimp;
using AppCode.RecaptchaValidator;
using AppCode.Mail;
using System.Linq;
using ToSic.Sxc.WebApi;

[AllowAnonymous]	// define that all commands can be accessed without a login
public class FormController : Custom.Hybrid.ApiTyped
{
  [HttpPost]
  [SecureEndpoint]
  public void ProcessForm([FromBody] SaveRequest contactFormRequest)
  {
    // Copy the data into a new variable, as only this will be sent per Mail and the Other Data is need to Save in the 2sxc
    var fieldsFormRequest = HtmlEncodeFields(contactFormRequest.Fields);
    var wrapLog = Log.Call(useTimer: true);
    var formConfig = As<FormConfig>(MyItem);

    if (formConfig.Recaptcha)
    {
      Log.Add("checking Recaptcha");
      GetService<Recaptcha>().Validate(contactFormRequest.Recaptcha);
    }

    // Same the TechnicalValues
    Dictionary<string, object> formTechnicalValues = new Dictionary<string, object>();

    // 1. add IP / host, and save all fields
    // if you add fields to your content-type, just make sure they are
    // in the request with the correct name, they will be added automatically
    formTechnicalValues["Timestamp"] = DateTime.Now;
    // Add the SenderIP in case we need to track down abuse
#if NETCOREAPP
    formTechnicalValues["SenderIP"] = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
#else
    formTechnicalValues["SenderIP"] = System.Web.HttpContext.Current.Request.UserHostAddress;
#endif
    // Add the ModuleId to assign each sent form to a specific module
    formTechnicalValues["ModuleId"] = MyContext.Module.Id;
    // Add the FormId to assign each sent form to a specific Form
    formTechnicalValues["FormId"] = contactFormRequest.Fields["FormId"].ToString();
    // add raw-data, in case the content-type has a "RawData" field
    contactFormRequest.Fields.Add("RawData", CreateRawDataEntry(contactFormRequest));

    // add Title (if non given), in case the content-type would benefit of an automatic title
    var addTitle = !contactFormRequest.Fields.ContainsKey("Title");
    if (addTitle) contactFormRequest.Fields.Add("Title", "Form " + DateTime.Now.ToString("s"));
    // Automatically full-save each request into a system-protocol content-type
    // This helps to debug or find submissions in case something wasn't configured right

    var contentType = MyItem.Bool("ReuseConfig") ? MyItem.Child("InheritedConfig").String("SaveToContentType") : MyItem.String("SaveToContentType");
    if (ToSic.Razor.Blade.Text.Has(MyItem.String("SaveToContentType")))
    {
      var contentTypeEntity = App.Data.Create(contentType, contactFormRequest.Fields);
    }

    // Create Fields Data
    var formDataEntity = App.Data.Create("FormData", contactFormRequest.Fields);
    // Update (Update to the same Entity) formTechnicalValues
    App.Data.Update(formDataEntity.EntityId, formTechnicalValues);

    Log.Add("Save data to content type");
    var files = new List<ToSic.Sxc.Adam.IFile>();

    // Save files to Adam
    contactFormRequest.Files?
    .Select(fileObj =>
    {
      var stream = new MemoryStream(fileObj.Contents);
      return SaveInAdam(
          stream: stream,
          fileName: fileObj.Name,
          contentType: "FormData",
          guid: formDataEntity.EntityGuid,
          field: "Files"
      );
    })
    .ToList()
    .ForEach(savedFile => files.Add(savedFile));

    if (contactFormRequest.Files == null || !contactFormRequest.Files.Any())
      Log.Add("No files found to save");


    if (formConfig.EnableMailchimp)
    {
      Log.Add("Mailchimp enabled"); 
      GetService<MailChimp>().Subscribe(contactFormRequest.Fields, formConfig.MailchimpSubscriptionMail, formConfig.MailchimpTagConfig);// int.TryParse(id, out var intId) ? intId : 0);
    }

    // Sending Mails
    GetService<Mail>().SendMails(fieldsFormRequest, contactFormRequest.CustomerMails, files);

    wrapLog("ok");
  }

  private object CreateRawDataEntry(SaveRequest formRequest)
  {
    var data = new Dictionary<string, object>();
    data.Add("Fields", formRequest.Fields);
    data.Add("Terms", formRequest.Terms);
    return Kit.Json.ToJson(data);
  }

  private static Dictionary<string, object> HtmlEncodeFields(Dictionary<string, object> dictionary)
  {
      var newDic = dictionary.ToDictionary(
          kvp => kvp.Key,
          kvp => {
              // Try to convert to string, if it fails, it's probably not a string
              var value = kvp.Value as string;
              if (value == null)
                  return kvp.Value;

              // HTML-Encode the string value
              return (object)HttpUtility.HtmlEncode(value);
          },
          StringComparer.OrdinalIgnoreCase
      );
      return newDic;
  }  
}


public class SaveRequest
{
  public Dictionary<string, object> Fields { get; set; }
  public List<FileUpload> Files { get; set; }
  public Dictionary<string, string> Terms { get; set; }
  public string Recaptcha { get; set; }
  public bool MailChimp { get; set; }
  public string CustomerMails { get; set; }
}

public class FileUpload
{
  public string Field { get; set; }
  public string Name { get; set; }
  public string Encoded { get; set; }
  public byte[] Contents { get { return System.Convert.FromBase64String(Encoded.Split(',')[1]); } }
}
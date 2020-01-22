using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Mail;

public class SendMail : ToSic.Sxc.Dnn.DynamicCode
{
  public bool Send(
    string emailTemplateFilename,
    Dictionary<string,object> valuesWithMailLabels,
    string MailFrom,
    string MailTo,
    string MailCC,
    string MailReply,
    List<ToSic.Sxc.Adam.IFile> files,
    ToSic.Sxc.Dnn.ApiController context)
  {
    // Log what's happening in case we run into problems
    var Log = context.Log; // this is a workaround, because 2sxc 10.25.02 didn't put the Log object on DynamicCode
    var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + MailFrom + ", to:" + MailTo + ", cc:" + MailCC + ", reply:" + MailReply);
    
		// Check for attachments and add them to the mail
		var attachments = files.Select(f =>
				new System.Net.Mail.Attachment(
					new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + f.Url, FileMode.Open), f.FullName)).ToList();

    Log.Add("Get MailEngine");
		var mailEngine = TemplateInstance(emailTemplateFilename, context.App.Path);
		var mailBody = mailEngine.Message(valuesWithMailLabels, context).ToString();
		var mailSubj = mailEngine.Subject(valuesWithMailLabels, context);

		// Send Mail
		// uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
    Log.Add("sending...");
		var sendMailResult = Mail.SendMail(MailFrom, MailTo,	MailCC,	"", MailReply, MailPriority.Normal,	mailSubj, MailFormat.Html, System.Text.Encoding.UTF8, mailBody, attachments, "", "", "", "", DotNetNuke.Entities.Host.Host.EnableSMTPSSL);

// TODO: 2ro - discuss, maybe not needed any more, as we can just add it to the EAV log?
    var logInfo = new DotNetNuke.Services.Log.EventLog.LogInfo
    {
        LogTypeKey = DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT.ToString()
    };
    logInfo.AddProperty("MailFrom", MailFrom);
    logInfo.AddProperty("MailTo", MailTo);
    logInfo.AddProperty("MailCC", MailCC);
    logInfo.AddProperty("MailReply", MailReply);
    logInfo.AddProperty("MailSubject", mailSubj);
    logInfo.AddProperty("SSL", DotNetNuke.Entities.Host.Host.EnableSMTPSSL.ToString());
    logInfo.AddProperty("Result", sendMailResult);
    DotNetNuke.Services.Log.EventLog.EventLogController.Instance.AddLog(logInfo);

    wrapLog("ok");
    return sendMailResult == "";
  }

  // get email template
	private dynamic TemplateInstance(string fileName, string AppPath)
  {
    var path = System.IO.Path.Combine("~", CreateInstancePath, "../../email-templates", fileName);
    var compiledType = BuildManager.GetCompiledType(path);

    if (compiledType == null)
      throw new Exception("Error while creating mail template instance.");
  
    var objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
    return ((dynamic)objectValue);
  }
}
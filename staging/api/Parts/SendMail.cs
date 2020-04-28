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
    List<ToSic.Sxc.Adam.IFile> files)
  {
    // Log what's happening in case we run into problems
    var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + MailFrom + ", to:" + MailTo + ", cc:" + MailCC + ", reply:" + MailReply);
    
		// Check for attachments and add them to the mail
		var attachments = files.Select(f =>
				new System.Net.Mail.Attachment(
					new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + f.Url, FileMode.Open), f.FullName)).ToList();

    Log.Add("Get MailEngine");
		// old 2sxc var mailEngine = TemplateInstance(emailTemplateFilename, context.App.Path);
    var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
		var mailBody = mailEngine.Message(valuesWithMailLabels, this).ToString();
		var mailSubj = mailEngine.Subject(valuesWithMailLabels, this);

		// Send Mail
		// uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
    Log.Add("sending...");
		var sendMailResult = Mail.SendMail(MailFrom, MailTo,	MailCC,	"", MailReply, MailPriority.Normal,	mailSubj, MailFormat.Html, System.Text.Encoding.UTF8, mailBody, attachments, "", "", "", "", DotNetNuke.Entities.Host.Host.EnableSMTPSSL);

    // Log to DNN - just as a last resort in case something is lost, to track down why
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
  
  // // get email template - before 10.28 update
  // keep for a while, in case somebody needs this code to downgrade to a prev. version of 2sxc
	// private dynamic TemplateInstance(string fileName, string AppPath)
  // {
  //   var path = System.IO.Path.Combine("~", CreateInstancePath, "../../email-templates", fileName);
  //   var compiledType = BuildManager.GetCompiledType(path);

  //   if (compiledType == null)
  //     throw new Exception("Error while creating mail template instance.");
  
  //   var objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
  //   return ((dynamic)objectValue);
  // }
}
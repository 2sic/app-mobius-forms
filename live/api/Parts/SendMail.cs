using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Mail;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;

public class SendMail : ToSic.Sxc.Dnn.DynamicCode
{
  public void sendMails(Dictionary<string,object> contactFormRequest, string workflowId, dynamic files) {
    var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
		var workflow = AsList(App.Data["Workflow"]).Where(w => w.WorkflowId == workflowId).FirstOrDefault();

		// rewrite the keys to be a nicer format, based on the configuration
		var valuesWithMailLabels = RewriteKeys(contactFormRequest, workflow.MailLabels ?? "");

    // assemble all settings to send the mail
		// background: some settings are made in this module,
		// but if they are missing we use fallback settings 
		// which are taken from the App.Settings
    var settings = new {
			MailFrom = !String.IsNullOrEmpty(Content.MailFrom) ? Content.MailFrom : App.Settings.DefaultMailFrom,
			OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : App.Settings.DefaultOwnerMail
		};

		// Send Mail to owner
		if(Content.OwnerSend != null && Content.OwnerSend) {
			Log.Add("Send Mail to Owner");
			try {
				Send(
					workflow.OwnerMailTemplate, valuesWithMailLabels, settings.MailFrom, settings.OwnerMail, Content.OwnerMailCC, custMail, files
				);
			} catch(Exception ex) {
				throw new Exception("OwnwerSend mail failed: " + ex.Message);
			}
		}

		// Send Mail to customer
		if(Content.CustomerSend != null && Content.CustomerSend && !String.IsNullOrEmpty(custMail)) {
			Log.Add("Send Mail to Customer");
			try {
				Send(
					workflow.CustomerMailTemplate, valuesWithMailLabels, settings.MailFrom, custMail, Content.CustomerMailCC, settings.OwnerMail, files
				);
			} catch(Exception ex) {
				throw new Exception("Customer Send mail failed: " + ex.Message);
			}
		}
  }

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
    var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
		var mailBody = mailEngine.Message(valuesWithMailLabels).ToString();
		var mailSubj = mailEngine.Subject(valuesWithMailLabels);

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

  // rewrite the keys to be a nicer format, based on the configuration
	private Dictionary<string, object> RewriteKeys(Dictionary<string, object> dic, string map)
	{
		// create keys-map
		Dictionary<string, string> newKeys = map
			.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
			.ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1], StringComparer.OrdinalIgnoreCase);

		return dic.ToDictionary(g => newKeys.ContainsKey(g.Key) ? newKeys[g.Key] : g.Key, g => g.Value, StringComparer.OrdinalIgnoreCase);
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class SendMail : Custom.Hybrid.CodeTyped
{
  public void SendMails(Dictionary<string, object> contactFormRequest, string workflowId, List<ToSic.Sxc.Adam.IFile>  files)

  {
    var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
    var workflow = AsItems(App.Data["Workflow"]).Where(w => w.String("WorkflowId") == workflowId).FirstOrDefault();

    // rewrite the keys to be a nicer format, based on the configuration
    var valuesRelabled = RewriteKeys(contactFormRequest, workflow.String("MailLabels") ?? "");

    // assemble all settings to send the mail
    // background: some settings are made in this module, but if they are missing we use fallback settings
    var formConfig = MyItem;
    var from = Text.First(formConfig.String("MailFrom"), App.Settings.String("DefaultMailFrom"));
    var owner = Text.First(formConfig.String("OwnerMail"), App.Settings.String("DefaultOwnerMail"));

    // Send Mail to owner
    if (formConfig.Bool("OwnerSend"))
    {
      Log.Add("Send Mail to Owner");
      try
      {
        Send(formConfig, workflow.String("OwnerMailTemplate"), valuesRelabled, from, owner, formConfig.String("OwnerMailCC"), custMail, files);
      }
      catch (Exception ex)
      {
        throw new Exception("OwnerSend mail failed: " + ex.Message);
      }
    }

    // Send Mail to customer
    if (formConfig.Bool("CustomerSend") && Text.Has(custMail))
    {
      Log.Add("Send Mail to Customer");
      try
      {
        Send(formConfig, workflow.String("CustomerMailTemplate"), valuesRelabled, from, custMail, formConfig.String("CustomerMailCC"), owner, files);
      }
      catch (Exception ex)
      {
        throw new Exception("Customer Send mail failed: " + ex.Message);
      }
    }
  }

  public void Send(ITypedItem formConfig, string emailTemplateFilename, Dictionary<string, object> valuesWithMailLabels, 
    string from, string to, string cc, string replyTo, List<ToSic.Sxc.Adam.IFile> files)
  {
    // Log what's happening in case we run into problems
    var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + from + ", to:" + to + ", cc:" + cc + ", reply:" + replyTo);

    Log.Add("Get MailEngine");
    var mailEngine = GetCode("../../email-templates/" + emailTemplateFilename);
    var mailBody = mailEngine.Message(formConfig, valuesWithMailLabels).ToString();
    var subject = mailEngine.Subject(formConfig, valuesWithMailLabels);

    // Send Mail
    // Note that if an error occurs, this will bubble up, the caller will convert it to format for the client
    Log.Add("sending...");
    Kit.Mail.Send(from: from, to: to, cc: cc, replyTo: replyTo, subject: subject, body: mailBody, attachments: files);

    // Log to Platform - just as a last resort in case something is lost, to track down why
    var message = new StringBuilder()
      .AppendLine("Send Mail")
      .AppendLine("From:    " + from)
      .AppendLine("To:      " + to)
      .AppendLine("CC:      " + cc)
      .AppendLine("Reply:   " + replyTo)
      .AppendLine("Subject: " + subject)
      .ToString();

    Kit.SystemLog.Add("SendMail", message);

    wrapLog("ok");
  }

  // rewrite the keys to be a nicer format, based on the configuration
  private Dictionary<string, object> RewriteKeys(Dictionary<string, object> dic, string map)
  {
    // create keys-map
    Dictionary<string, string> newKeys = map
      .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
      .ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1], StringComparer.OrdinalIgnoreCase);

    return dic.ToDictionary(g => newKeys.ContainsKey(g.Key) ? newKeys[g.Key] : g.Key, g => g.Value, StringComparer.OrdinalIgnoreCase);
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services; // platformLogService, mailService
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;

public class SendMail : Custom.Hybrid.Code12
{
  public void SendMails(Dictionary<string, object> contactFormRequest, string workflowId, dynamic files)
  {
    var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
    var workflow = AsList(App.Data["Workflow"]).Where(w => w.WorkflowId == workflowId).FirstOrDefault();

    // rewrite the keys to be a nicer format, based on the configuration
    var valuesRelabled = RewriteKeys(contactFormRequest, workflow.MailLabels ?? "");

    // assemble all settings to send the mail
    // background: some settings are made in this module, but if they are missing we use fallback settings
    var from = Text.Has(Content.MailFrom) ? Content.MailFrom : Settings.DefaultMailFrom;
    var owner = Text.Has(Content.OwnerMail) ? Content.OwnerMail : Settings.DefaultOwnerMail;

    // Send Mail to owner
    if (Content.OwnerSend == true)
    {
      Log.Add("Send Mail to Owner");
      try
      {
        Send(workflow.OwnerMailTemplate, valuesRelabled, from, owner, Content.OwnerMailCC, custMail, files);
      }
      catch (Exception ex)
      {
        throw new Exception("OwnerSend mail failed: " + ex.Message);
      }
    }

    // Send Mail to customer
    if (Content.CustomerSend == true && Text.Has(custMail))
    {
      Log.Add("Send Mail to Customer");
      try
      {
        Send(workflow.CustomerMailTemplate, valuesRelabled, from, custMail, Content.CustomerMailCC, owner, files);
      }
      catch (Exception ex)
      {
        throw new Exception("Customer Send mail failed: " + ex.Message);
      }
    }
  }

  public void Send(string emailTemplateFilename, Dictionary<string, object> valuesWithMailLabels, 
    string from, string to, string cc, string replyTo, List<ToSic.Sxc.Adam.IFile> files)
  {
    // Log what's happening in case we run into problems
    var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + from + ", to:" + to + ", cc:" + cc + ", reply:" + replyTo);

    Log.Add("Get MailEngine");
    var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
    var mailBody = mailEngine.Message(valuesWithMailLabels).ToString();
    var subject = mailEngine.Subject(valuesWithMailLabels);

    // Send Mail
    // Note that if an error occurs, this will bubble up, the caller will convert it to format for the client
    Log.Add("sending...");
    var mailService = GetService<IMailService>();
    mailService.Send(from: from, to: to, cc: cc, replyTo: replyTo, subject: subject, body: mailBody, attachments: files);

    // Log to Platform - just as a last resort in case something is lost, to track down why
    var message = new StringBuilder()
      .AppendLine("Send Mail")
      .AppendLine("From:    " + from)
      .AppendLine("To:      " + to)
      .AppendLine("CC:      " + cc)
      .AppendLine("Reply:   " + replyTo)
      .AppendLine("Subject: " + subject)
      .ToString();

    GetService<ILogService>().Add("SendMail", message);

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

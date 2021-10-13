using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
using ToSic.Sxc.Services; // platformLogService, mailService

public class SendMail : Custom.Hybrid.Code12
{
    public void sendMails(Dictionary<string, object> contactFormRequest, string workflowId, dynamic files)
    {
        var custMail = contactFormRequest.ContainsKey("SenderMail") ? contactFormRequest["SenderMail"].ToString() : "";
        var workflow = AsList(App.Data["Workflow"]).Where(w => w.WorkflowId == workflowId).FirstOrDefault();

        // rewrite the keys to be a nicer format, based on the configuration
        var valuesWithMailLabels = RewriteKeys(contactFormRequest, workflow.MailLabels ?? "");

        // assemble all settings to send the mail
        // background: some settings are made in this module,
        // but if they are missing we use fallback settings
        // which are taken from the Settings
        var settings = new
        {
            MailFrom = !String.IsNullOrEmpty(Content.MailFrom) ? Content.MailFrom : Settings.DefaultMailFrom,
            OwnerMail = !String.IsNullOrEmpty(Content.OwnerMail) ? Content.OwnerMail : Settings.DefaultOwnerMail
        };

        // Send Mail to owner
        if (Content.OwnerSend != null && Content.OwnerSend)
        {
            Log.Add("Send Mail to Owner");
            try
            {
                Send(
                    workflow.OwnerMailTemplate, valuesWithMailLabels, settings.MailFrom, settings.OwnerMail, Content.OwnerMailCC, custMail, files
                );
            }
            catch (Exception ex)
            {
                throw new Exception("OwnerSend mail failed: " + ex.Message);
            }
        }

        // Send Mail to customer
        if (Content.CustomerSend != null && Content.CustomerSend && !String.IsNullOrEmpty(custMail))
        {
            Log.Add("Send Mail to Customer");
            try
            {
                Send(
                    workflow.CustomerMailTemplate, valuesWithMailLabels, settings.MailFrom, custMail, Content.CustomerMailCC, settings.OwnerMail, files
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Customer Send mail failed: " + ex.Message);
            }
        }
    }

    public void Send(
        string emailTemplateFilename,
        Dictionary<string, object> valuesWithMailLabels,
        string MailFrom,
        string MailTo,
        string MailCC,
        string MailReply,
        List<ToSic.Sxc.Adam.IFile> files)
    {
        // Log what's happening in case we run into problems
        var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + MailFrom + ", to:" + MailTo + ", cc:" + MailCC + ", reply:" + MailReply);

        Log.Add("Get MailEngine");
        var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
        var mailBody = mailEngine.Message(valuesWithMailLabels).ToString();
        var mailSubj = mailEngine.Subject(valuesWithMailLabels);

        // Send Mail
        // TODO: @2ro - what do we send the browser if an error occurs? must verify
        Log.Add("sending...");
        var mailService = GetService<IMailService>();
        mailService.Send(
            from: MailFrom,
            to: MailTo,
            cc: MailCC,
            bcc: "",
            replyTo: MailReply,
            subject: mailSubj,
            body: mailBody,
            attachments: files);

        // Log to Platform - just as a last resort in case something is lost, to track down why
        var message = new StringBuilder();
        message.AppendFormat("{0}: {1}{2}", "MailFrom", MailFrom, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailTo", MailTo, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailCC", MailCC, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailReply", MailReply, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailSubject", mailSubj, Environment.NewLine);
        //message.AppendFormat("{0}: {1}{2}", "Result", "", Environment.NewLine);

        var platformLogService = GetService<ILogService>();
        platformLogService.Add("SendMail", message.ToString());

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

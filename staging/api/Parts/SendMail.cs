#if NETCOREAPP // Oqtane
// using Microsoft.Extensions.DependencyInjection;
#else // DNN
// 2sxclint:disable:no-dnn-namespaces
// using System.Runtime.CompilerServices;
// using DotNetNuke.Services.Mail;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
using ToSic.Sxc.Services; // platformLogService
using ToSic.Sxc.Web; // mailService

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

    public bool Send(
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

        // Check for attachments and add them to the mail
        var attachments = files.Select(f =>
                new System.Net.Mail.Attachment(
                    new FileStream(f.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read), f.FullName)).ToList();

        Log.Add("Get MailEngine");
        var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
        var mailBody = mailEngine.Message(valuesWithMailLabels).ToString();
        var mailSubj = mailEngine.Subject(valuesWithMailLabels);

        // Send Mail
        Log.Add("sending...");

        // get services
        var mailService = GetService<IMailService>();
        var sendMailResult = mailService.Send(
            mailFrom: MailFrom,
            mailTo: MailTo,
            cc: MailCC,
            bcc: "",
            replyTo: MailReply,
            priority: MailPriority.Normal,
            subject: mailSubj,
            isBodyHtml: true,
            bodyEncoding: Encoding.UTF8,
            body: mailBody,
            attachments: attachments);

        // Log to Platform - just as a last resort in case something is lost, to track down why
        var message = new StringBuilder();
        message.AppendFormat("{0}: {1}{2}", "MailFrom", MailFrom, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailTo", MailTo, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailCC", MailCC, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailReply", MailReply, Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "MailSubject", mailSubj, Environment.NewLine);
        // message.AppendFormat("{0}: {1}{2}", "SSL", DotNetNuke.Entities.Host.Host.EnableSMTPSSL.ToString(), Environment.NewLine);
        message.AppendFormat("{0}: {1}{2}", "Result", sendMailResult, Environment.NewLine);

        // get services
        var platformLogService = GetService<ILogService>();
        platformLogService.Add("SendMail", message.ToString());

        wrapLog("ok");
        return sendMailResult == "";
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

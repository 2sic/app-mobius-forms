using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
#if NETCOREAPP // Oqtane
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
#else // DNN
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Mail;
#endif

// TODO: file attachments are not working, because are not provided by form.
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
                    // TODO: 2dm
                    // new FileStream(Link.To(api: f.Url, type: "full"), FileMode.Open), f.FullName)).ToList();
                    new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + f.Url, FileMode.Open), f.FullName)).ToList();

        Log.Add("Get MailEngine");
        var mailEngine = CreateInstance("../../email-templates/" + emailTemplateFilename);
        var mailBody = mailEngine.Message(valuesWithMailLabels).ToString();
        var mailSubj = mailEngine.Subject(valuesWithMailLabels);

        // Send Mail
        Log.Add("sending...");

#if NETCOREAPP // Oqtane
        var sendMailResult = OqtSendMail(MailFrom, MailTo, MailCC, "", MailReply, MailPriority.Normal, mailSubj, true, Encoding.UTF8, mailBody, attachments);
#else // DNN
        // uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
        var sendMailResult = Mail.SendMail(MailFrom, MailTo, MailCC, "", MailReply, MailPriority.Normal, mailSubj, MailFormat.Html, Encoding.UTF8, mailBody, attachments, "", "", "", "", DotNetNuke.Entities.Host.Host.EnableSMTPSSL);

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
#endif

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

#if NETCOREAPP // Oqtane
    private string OqtSendMail(
        string mailFrom,
        string mailTo,
        string cc,
        string bcc,
        string replyTo,
        MailPriority priority,
        string subject,
        bool isBodyHtml,
        Encoding bodyEncoding,
        string body,
        List<Attachment> attachments
    )
    {
        string log = "";

        // get services
        var siteRepository = GetService<ISiteRepository>();
        var userRepository = GetService<IUserRepository>();
        var settingRepository = GetService<ISettingRepository>();
        var notificationRepository = GetService<INotificationRepository>();

        Site site = siteRepository.GetSite(CmsContext.Site.Id);

        // get site settings
        List<Setting> sitesettings = settingRepository.GetSettings(EntityNames.Site, site.SiteId).ToList();
        Dictionary<string, string> settings = GetSettings(sitesettings);
        if (settings.ContainsKey("SMTPHost") && settings["SMTPHost"] != "" &&
            settings.ContainsKey("SMTPPort") && settings["SMTPPort"] != "" &&
            settings.ContainsKey("SMTPSSL") && settings["SMTPSSL"] != "" &&
            settings.ContainsKey("SMTPSender") && settings["SMTPSender"] != "")
        {
            // send mail
            try
            {
                // construct SMTP Client
                var client = new SmtpClient()
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = settings["SMTPHost"],
                    Port = int.Parse(settings["SMTPPort"]),
                    EnableSsl = bool.Parse(settings["SMTPSSL"])
                };

                if (settings["SMTPUsername"] != "" && settings["SMTPPassword"] != "")
                {
                    client.Credentials = new NetworkCredential(settings["SMTPUsername"], settings["SMTPPassword"]);
                }

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailFrom); // 1
                AddMailAddresses(mailMessage.To, mailTo); // 2 - what if we have more emails
                AddMailAddresses(mailMessage.CC, cc); // 3 - what if we have more emails
                AddMailAddresses(mailMessage.Bcc, bcc);
                if (!string.IsNullOrEmpty(replyTo)) mailMessage.ReplyTo = new MailAddress(replyTo); // 5
                mailMessage.Priority = priority; // 6
                mailMessage.Subject = subject; // 7
                mailMessage.IsBodyHtml = isBodyHtml; // 8
                mailMessage.BodyEncoding = bodyEncoding; // 9
                mailMessage.Body = body; // 10

                foreach (var attachment in attachments) // 11
                {
                    mailMessage.Attachments.Add(attachment);
                }

                client.Send(mailMessage);
                log += "OK: Email sent. ";
            }
            catch (Exception ex)
            {
                // error
                log += $"Error: {ex.Message}. ";
            }
        }
        else
        {
            log += "SMTP Not Configured Properly In Site Settings - Host, Port, SSL, And Sender Are All Required.";
        }
        return log;
    }

    private Dictionary<string, string> GetSettings(List<Setting> settings)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (Setting setting in settings.OrderBy(item => item.SettingName).ToList())
        {
            dictionary.Add(setting.SettingName, setting.SettingValue);
        }
        return dictionary;
    }

    private void AddMailAddresses(MailAddressCollection mails, string emails)
    {
        if (string.IsNullOrEmpty(emails)) return;
        foreach (var email in emails.Split(",;".ToCharArray()))
        {
            mails.Add(new MailAddress(email));
        }
    }
#endif

}

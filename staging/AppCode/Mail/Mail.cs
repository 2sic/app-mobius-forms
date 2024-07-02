using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToSic.Razor.Blade;

using AppCode.Data;

namespace AppCode.Mail
{
  public class Mail : Custom.Hybrid.CodeTyped
  {
    public void SendMails(Dictionary<string, object> contactFormRequest, string customerMails, List<ToSic.Sxc.Adam.IFile> files)
    {
      // rewrite the keys to be a nicer format, based on the configuration
      var mailLabels = "";

      if (contactFormRequest.ContainsKey("FormId"))
      {
        var dynForm = AsItems(App.Data["FormConfig"]).Where(e => e.Id == Int32.Parse(contactFormRequest["FormId"].ToString())).FirstOrDefault();

        foreach (var item in dynForm.Children("Fields"))
        {
          var fieldId = item.String("FieldId");
          var title = item.String("Title");
          mailLabels += fieldId + "=" + title + "\n";
        }
      }

      var valuesRelabled = RewriteKeys(contactFormRequest, mailLabels);

      // assemble all settings to send the mail
      // background: some settings are made in this module, but if they are missing we use fallback settings$

      var appSettings = As<AppSettings>(App.Settings);
      var formConfig = As<FormConfig>(MyItem);
      var sendMailConfig = formConfig.FormSendMailConfig; // SendMailConfig; // MyItem.Bool("ReuseConfig") ? MyItem.Child("InheritedConfig").Child("FormSendMailConfig") : MyItem.Child("FormSendMailConfig");

      var dataStacker = GetService<DataStackHelper>();
      var sendMailConfigStack = dataStacker.GetSendMail(sendMailConfig);

      var from = Text.First(sendMailConfigStack.MailFrom, appSettings.DefaultMailFrom);
      var owner = Text.First(sendMailConfigStack.OwnerMail, appSettings.DefaultOwnerMail);

      // If Mail Settings are missing, throw an exception
      if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(owner))
        throw new Exception("Mail settings are missing. Please configure 'MailFrom' and 'OwnerMail' settings.");

      // Send Mail to owner
      if (sendMailConfigStack.OwnerSend)
      {
        Log.Add("Send Mail to Owner");
        try
        {
          Send(sendMailConfigStack, sendMailConfigStack.OwnerMailTemplate, valuesRelabled, from, owner, sendMailConfigStack.OwnerMailCC, customerMails, files);
        }
        catch (Exception ex)
        {
          throw new Exception("OwnerSend mail failed: " + ex.Message);
        }
      }

      // Send Mail to customer
      if (sendMailConfigStack.CustomerSend && Text.Has(customerMails))
      {
        Log.Add("Send Mail to Customer");
        try
        {
          Send(sendMailConfigStack, sendMailConfigStack.CustomerMailTemplate, valuesRelabled, from, customerMails, sendMailConfigStack.CustomerMailCC, owner, files);
        }
        catch (Exception ex)
        {
          throw new Exception("Customer Send mail failed: " + ex.Message);
        }
      }
    }

    public void Send(SendMailConfigStack sendMailConfig, string emailTemplateFilename, Dictionary<string, object> valuesWithMailLabels,
      string from, string to, string cc, string replyTo, List<ToSic.Sxc.Adam.IFile> files)
    {
      // Log what's happening in case we run into problems
      var wrapLog = Log.Call("template:" + emailTemplateFilename + ", from:" + from + ", to:" + to + ", cc:" + cc + ", reply:" + replyTo);

      Log.Add("Get MailEngine");
      var mailEngine = GetService<IMailTemplate>(typeName: "AppCode.MailTemplates." + emailTemplateFilename.Replace(".cs", ""));

      var stackHelper = GetService<DataStackHelper>();
      var formResources = stackHelper.GetFormResources(As<FormConfig>(MyItem)); // TODO:: auf FormConfig umstellen

      var subject = mailEngine.Subject(formResources);
      var mailBody = mailEngine.Message(formResources, valuesWithMailLabels).ToString();

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
}

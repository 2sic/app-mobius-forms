using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using DotNetNuke.Services.Mail;

public class SendMail
{
  public bool send(
    string emailTemplateFilename,
    Dictionary<string,object> valuesWithMailLabels,
    string MailFrom,
    string MailTo,
    string MailCC,
    string MailReply,
    List<ToSic.Sxc.Adam.IFile> files,
    ToSic.SexyContent.IAppAndDataHelpers context)
  {
		// Check for attachments and add them to the mail
		var attachments = files.Select(f =>
				new System.Net.Mail.Attachment(
					new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + f.Url, FileMode.Open), f.FullName)).ToList();

		var mailEngine = TemplateInstance(emailTemplateFilename, context.App.Path);
		var mailBody = mailEngine.Message(valuesWithMailLabels, context).ToString();
		var mailSubj = mailEngine.Subject(valuesWithMailLabels, context);

		// Send Mail
		// uses the DNN command: http://www.dnnsoftware.com/dnn-api/html/886d0ac8-45e8-6472-455a-a7adced60ada.htm
		Mail.SendMail(MailFrom, MailTo,	MailCC,	"", MailReply, MailPriority.Normal,	mailSubj, MailFormat.Html, System.Text.Encoding.UTF8, mailBody, attachments, "", "", "", "", false);

    return true;
  }

  // get email template
	private dynamic TemplateInstance(string fileName, string AppPath)
	{
		var path = System.IO.Path.Combine("~", AppPath, getEdition() , "email-templates", fileName);
		var compiledType = BuildManager.GetCompiledType(path);
		
		object objectValue = null;	
		if (compiledType != null)
		{
			objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
			return ((dynamic)objectValue);
		}
		throw new Exception("Error while creating mail template instance.");
	}

  // Get current edition (live/staging)
	private string getEdition(){
		var path = HttpContext.Current.Request.Url.AbsolutePath;
		return path.IndexOf("staging") > 0 ? "staging" : "live";
	}
}
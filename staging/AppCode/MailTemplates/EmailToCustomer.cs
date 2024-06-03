using System.Collections.Generic;
using AppCode.Data;
using AppCode.Mail;

namespace AppCode.MailTemplates
{
  public class EmailToCustomer : Custom.Hybrid.CodeTyped, IMailTemplate
  {
    // create custom subject here
    public string Subject(FormResourcesStack formResources)
    {
      // create custom code to generate the subject here...or just return the setting configured in the form
      return Kit.Scrub.Only(formResources.CustomerMailSubject, "p");
    }

    public string Message(FormResourcesStack formResources, Dictionary<string, object> data)
    {

      var message = @"
      <!doctype html>
      <html>
      <head>
          <meta name='viewport' content='width=device-width'>
          <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
          <style type='text/css'>
              body { font-family: Helvetica, sans-serif; }
          </style>
      </head>
      <body>
          <div>"
              + formResources.MailBodyCustomer +
          @"</div>
      </body>
      </html>";

      return message;
    }
  }
}
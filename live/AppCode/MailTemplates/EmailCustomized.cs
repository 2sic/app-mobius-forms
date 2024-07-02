using System.Collections.Generic;
using AppCode.Data;
using AppCode.Mail;

namespace AppCode.MailTemplates
{
  public class EmailCustomized : Custom.Hybrid.CodeTyped, IMailTemplate
    {
        // create custom subject here
        public string Subject(FormResourcesStack formResources)
        {

            // create custom code to generate the subject here...or just return the setting configured in the form
            return Kit.Scrub.Only(formResources.OwnerMailSubject, "p");
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
            <h1>Website contact form request</h1>
            <table width='100%'>
                <tr>
                    <td width='10%'>Subject</td>
                    <td>" + data["Subject"] + @"</td>
                </tr>
                <tr>
                    <td>Message</td>
                    <td" + data["Message"] + @"</td>
                </tr>
                <tr>
                    <td>Customer</td>
                    <td>" + data["Name"] + " " + data["E-Mail"] + @"</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>

                <tr>
                    <td>Log</td>
                    <td>" + data["Timestamp"] + data["SenderIP"] + @"</td>
                </tr>
            </table>
        </body>
        </html>";

            return message;
        }
    }
}
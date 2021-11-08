using ToSic.Razor.Blade;
using System.Collections.Generic;
using System;
public class EmailToCustomer: Custom.Hybrid.Code12
{
  // create custom subject here
  public string Subject(dynamic data) {
    // create custom code to generate the subject here...or just return the setting configured in the form
    return Text.First(Content.CustomerMailSubject, Resources.CustomerMailSubject);
  }

  public string Message(Dictionary<string,object> data)
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
        <h1>" + Resources.MailCustomerTitle + @"</h1>
        <div>"
            + Resources.MailCustomerContent +
        @"</div>
    </body>
    </html>";
    
    return message;
  }
}
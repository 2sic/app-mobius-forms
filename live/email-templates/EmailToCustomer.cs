using System;
using System.Collections.Generic;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class EmailToCustomer: Custom.Hybrid.CodeTyped
{
  // create custom subject here
  public string Subject(ITypedItem formConfig, Dictionary<string, object> data) {
    // create custom code to generate the subject here...or just return the setting configured in the form
    return Text.First(formConfig.String("CustomerMailSubject"), App.Resources.String("CustomerMailSubject"));
  }

  public string Message(ITypedItem formConfig, Dictionary<string, object> data)
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
        <h1>" + App.Resources.String("MailCustomerTitle") + @"</h1>
        <div>"
            + App.Resources.String("MailCustomerContent") +
        @"</div>
    </body>
    </html>";
    
    return message;
  }
}
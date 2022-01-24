using ToSic.Razor.Blade;
using System.Collections.Generic;
using System;
public class EmailToCustomerWithData: Custom.Hybrid.Code12 
{
  // create custom subject here
  public string Subject(dynamic data) {
    // create custom code to generate the subject here...or just return the setting configured in the form
    return Text.First(Content.CustomerMailSubject, App.Resources.CustomerMailSubject);
  }

  public string Message(Dictionary<string,object> data) {
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
        <strong>" + data["Subject"] + @"</strong>
        <h1>" + Resources.MailCustomerTitle + @"</h1>
        <div>"
            + Resources.MailCustomerContent +
        @"</div>
        <br/>
        <div>" + Resources.MailCustomerContentWithData + @"</div>";

    foreach (var item in data)
    {
        message += @"
        <div>
            <strong>" + item.Key + "</strong>: " + item.Value +                    
        "</div>";
    }
    message += "</body></html>";
    
    return message;
  }
}
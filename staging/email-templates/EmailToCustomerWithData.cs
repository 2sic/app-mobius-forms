using System.Collections.Generic;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ThisApp.Data;
using ThisApp.Code;
using ThisApp;

public class EmailToCustomerWithData: Custom.Hybrid.CodeTyped
{
  // create custom subject here
  public string Subject(FormResources formResources) {
    // create custom code to generate the subject here...or just return the setting configured in the form
    return Kit.Scrub.Only(formResources.CustomerMailSubject, "p");
  }

  public string Message(AppResources appRes, FormResources formResources, Dictionary<string, object> data) {

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
        <div>"
            + formResources.MailBodyCustomer +
        @"</div>
        <br/>
        <div>" + appRes.MailCustomerContentWithData + @"</div>";

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
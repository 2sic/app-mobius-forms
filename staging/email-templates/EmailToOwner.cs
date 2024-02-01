using System.Collections.Generic;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ThisApp.Data;
using ThisApp.Code;
using ThisApp;

public class EmailToOwner: Custom.Hybrid.CodeTyped
{
  // create custom subject here
  public string Subject(DynForm dynFormConfig, Dictionary<string, object> data) {
    var appRes = As<AppResources>(App.Resources);


    // create custom code to generate the subject here...or just return the setting configured in the form
    return Text.First(dynFormConfig.OwnerMailSubject, appRes.OwnerMailSubject);
  }
  public string Message(DynForm dynFormConfig, Dictionary<string, object> data)
  {
    var appRes = As<AppResources>(App.Resources);
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
        <h1>" + appRes.MailOwnerTitle + @"</h1>
        <div>"
            + appRes.MailOwnerContent +
        "</div>";

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

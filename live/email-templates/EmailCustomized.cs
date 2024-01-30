using System.Collections.Generic;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ThisApp.Data;
using ThisApp.Code;
using ThisApp;

public class EmailCustomized: Custom.Hybrid.CodeTyped
{
  // create custom subject here
  public string Subject(DynForm dynFormConfig, Dictionary<string, object> data) {
    var appRes = new AppResources(App.Resources);
    
    // create custom code to generate the subject here...or just return the setting configured in the form
    return Text.First(dynFormConfig.OwnerMailSubject, appRes.OwnerMailSubject);
  }

  public string Message(DynForm dynFormConfig, Dictionary<string, object> data)
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
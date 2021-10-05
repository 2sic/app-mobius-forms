using ToSic.Razor.Blade;
using System.Collections.Generic;
using System;
public class EmailToOwner: Custom.Hybrid.Code12
{
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
            <h1>" + Resources.MailOwnerTitle + @"</h1>
            <div>"
                + Resources.MailOwnerContent +
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
	public string Subject(dynamic data) {
        // create custom code to generate the subject here...
        // or just return the setting configured in the form
        return !String.IsNullOrWhiteSpace(Content.OwnerMailSubject)
            ? Content.OwnerMailSubject.ToString()
            : Resources.OwnerMailSubject.ToString();
	}
}

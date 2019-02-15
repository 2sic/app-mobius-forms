using System;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http;
using ToSic.SexyContent.WebApi;
using System.Web.Compilation;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Services.Mail;
using Newtonsoft.Json;


public class MailchimpController : SxcApiController
{
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public string Subscribe(String email, String fname, String lname)
        {
            var msg = SubscribeToMailChimp(App.Settings.MailchimpServer, App.Settings.MailchimpListId, App.Settings.MailchimpAPIKey, App.Settings.MailchimpSubscriptionStatus, email, fname, lname);

            return msg;
        }

        private static string SubscribeToMailChimp(string srv, string listId, string apiKey, string substatus, string email, string fname, string lname)
        {
            var wr = WebRequest.Create("https://" + srv + ".api.mailchimp.com/3.0/lists/" + listId + "/members");
            wr.Method = "POST";
            wr.ContentType = "application/json; charset=utf-8";

            var user = "anystring";

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + apiKey));
            wr.Headers.Add("Authorization", "Basic " + encoded);

            // You will have to change these names "EMAIL", "FNAME", "LNAME" to the field names you have in your Mailchimp list
            var body = "{\"EMAIL\":\"" + email + "\", \"status\":\"" + substatus + "\", \"merge_fields\": { \"FNAME\":\"" + fname + "\", \"LNAME\": \"" + lname + "\" } }";
            var reqStream = wr.GetRequestStream();

            using (var sw = new StreamWriter(reqStream))
            {
                sw.Write(body);
            }
            reqStream.Close();

            var response = wr.GetResponse();

            var respStm = response.GetResponseStream();
            var swr = new StreamReader(respStm);
            return swr.ReadToEnd();
        }
}
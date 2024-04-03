using System.Collections.Generic;
using AppCode.Data;

namespace AppCode.Mail
{
    /// <summary>
    /// Interface describing MailTemplate classes which can provide subject and message.
    /// </summary>
    public interface IMailTemplate
    {
        /// <summary>
        /// Generate the subject for the mail.
        /// </summary>
        string Subject(FormResourcesStack formResources);

        /// <summary>
        /// Generate the message for the mail.
        /// </summary>
        public string Message(FormResourcesStack formResources, Dictionary<string, object> data);
    }
}
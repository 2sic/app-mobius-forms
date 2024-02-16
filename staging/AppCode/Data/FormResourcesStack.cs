using ToSic.Sxc.Data;

namespace AppCode.Data
{
  public class FormResourcesStack
  {
    public FormResourcesStack(ITypedStack formResources)
    {
      FormResources = formResources;
    }
    protected ITypedStack FormResources { get; }

    public string Title => FormResources.String(nameof(Title), fallback: "");

    #region GroupValidation
    public string LabelRequired => FormResources.String(nameof(LabelRequired), fallback: "");
    public string LabelSelect => FormResources.String(nameof(LabelSelect), fallback: "");
    public string LabelValidEmail => FormResources.String(nameof(LabelValidEmail), fallback: ""); // Remove ?
    public string LabelValidFile => FormResources.String(nameof(LabelValidFile), fallback: ""); // Remove ?
    #endregion

    #region GroupMessages
    public string MessageFormIncomplete => FormResources.String(nameof(MessageFormIncomplete), fallback: "");
    public string MessageSending => FormResources.String(nameof(MessageSending), fallback: "");
    public string MessageSendOk => FormResources.String(nameof(MessageSendOk), fallback: "");
    public string MessageCustomerMailSend => FormResources.String(nameof(MessageCustomerMailSend), fallback: "");
    public string MessageSendError => FormResources.String(nameof(MessageSendError), fallback: "");
    public string MessageNewsletterSuccess => FormResources.String(nameof(MessageNewsletterSuccess), fallback: "");
    public string MessageNewsletterUnexpectedError => FormResources.String(nameof(MessageNewsletterUnexpectedError), fallback: "");
    public string MessageMailFromError => FormResources.String(nameof(MessageMailFromError), fallback: "");
    public string MessageMailOwnerError => FormResources.String(nameof(MessageMailOwnerError), fallback: "");

    #endregion

    #region GroupButtons
    public string Button => FormResources.String(nameof(Button), fallback: "");
    public string ButtonNextStep => FormResources.String(nameof(ButtonNextStep), fallback: "");
    #endregion

    #region GroupMail
    public string MailCustomerContentWithData => FormResources.String(nameof(MailCustomerContentWithData), fallback: "");

    #endregion

    #region FileUpload
    public string MessageDisabledFeature => FormResources.String(nameof(MessageDisabledFeature), fallback: "");
    #endregion

    #region MailSettings
    public string OwnerMailSubject => FormResources.String(nameof(OwnerMailSubject), fallback: "");
    public string CustomerMailSubject => FormResources.String(nameof(CustomerMailSubject), fallback: "");
    public string MailBodyCustomer => FormResources.String(nameof(MailBodyCustomer), fallback: "");
    public string MailBodyOwner => FormResources.String(nameof(MailBodyOwner), fallback: "");

    #endregion

    #region GroupTerms

    public string LabelTermsAll => FormResources.String(nameof(LabelTermsAll), fallback: "");
    public string LabelTerms => FormResources.String(nameof(LabelTerms), fallback: "");
    public string LabelGdpr => FormResources.String(nameof(LabelGdpr), fallback: "");

    #endregion

  }

}
namespace ThisApp.Data
{
  public partial class AppResources : Custom.Data.Item16
  {
    public string ToolbarReuseInfo => String(fallback: "");
    public string LabelFromDataAvailable => String(fallback: "");

    #region Terms

    public string LabelTermsAll => String(fallback: "");
    public string LabelTerms => String(fallback: "");
    public string LabelGdpr => String(fallback: "");
    public string LabelSelect => String(fallback: "");

    #endregion

    #region Dynamic Data Table

    public string LabelFirst => String(fallback: "");
    public string LabelLast => String(fallback: "");
    public string LabelTimestamp => String(fallback: "");

    #endregion

    #region Toolbar

    public string ToolbarViewData => String(fallback: "");
    public string ToolbarConfigure => String(fallback: "");
    public string ToolbarAppResources => String(fallback: "");

    #endregion

    #region Mailchimp and Footer Mailchimp

    public string MessageEnableMailchimp => String(fallback: "");
    public string MessageContainsMailchimp => String(fallback: "");
    public string MessageDefaultMailChimpKey => String(fallback: "");

    #endregion

    #region Footer Submit with Message

    public string MessageFormIncomplete => String(fallback: "");
    public string MessageRecaptchaMissing => String(fallback: "");
    public string MessageSendError => String(fallback: "");
    public string MessageSendOk => String(fallback: "");

    public string MessageSending => String(fallback: "");
    public string MessageNewsletterSuccess => String(fallback: "");
    public string MessageNewsletterUnexpectedError => String(fallback: "");
    public string MessageCustomerMailSend => String(fallback: "");
    public string MessageMailFromError => String(fallback: "");
    public string MessageMailOwnerError => String(fallback: "");
    public string ButtonSend => String(fallback: "");

    #endregion

    #region Footer Recaptcha

    public string MessageContainsRecaptcha => String(fallback: "");
    public string MessageRecaptchaWarning => String(fallback: "");

    #endregion

    #region EmailToCustomized

    public string OwnerMailSubject => String(fallback: "");

    #endregion

    #region EmailToCustomer

    public string CustomerMailSubject => String(fallback: "");
    public string MailCustomerTitle => String(fallback: "");
    public string MailCustomerContent => String(fallback: "");

    #endregion

    #region EmailToCustomerWithData
    public string MailCustomerContentWithData => String(fallback: "");

    #endregion

    #region EmailToOwner
    public string MailOwnerTitle => String(fallback: "");
    public string MailOwnerContent => String(fallback: "");

    #endregion


  }
}
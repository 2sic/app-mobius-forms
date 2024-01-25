using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class AppResources : Custom.Data.Item16Experimental
  {
    public AppResources(ITypedItem item) : base(item) { }

    public string ToolbarReuseInfo => GetThis(fallback: "");
    public string LabelFromDataAvailable => GetThis(fallback: "");

    #region Terms

    public string LabelTermsAll => GetThis(fallback: "");
    public string LabelTerms => GetThis(fallback: "");
    public string LabelGdpr => GetThis(fallback: "");
    public string LabelSelect => GetThis(fallback: "");

    #endregion

    #region Dynamic Data Table

    public string LabelFirst => GetThis(fallback: "");
    public string LabelLast => GetThis(fallback: "");
    public string LabelTimestamp => GetThis(fallback: "");

    #endregion

    #region Toolbar

    public string ToolbarViewData => GetThis(fallback: "");
    public string ToolbarConfigure => GetThis(fallback: "");
    public string ToolbarAppResources => GetThis(fallback: "");

    #endregion

    #region Mailchimp and Footer Mailchimp
    
    public string MessageEnableMailchimp => GetThis(fallback: "");
    public string MessageContainsMailchimp => GetThis(fallback: "");
    public string MessageDefaultMailChimpKey => GetThis(fallback: "");

    #endregion

    #region Footer Submit with Message
    
    public string MessageFormIncomplete => GetThis(fallback: "");
    public string MessageRecaptchaMissing => GetThis(fallback: "");
    public string MessageSendError => GetThis(fallback: "");
    public string MessageSendOk => GetThis(fallback: "");

    public string MessageSending => GetThis(fallback: "");
    public string MessageNewsletterSuccess => GetThis(fallback: "");
    public string MessageNewsletterUnexpectedError => GetThis(fallback: "");
    public string MessageCustomerMailSend => GetThis(fallback: "");
    public string MessageMailFromError => GetThis(fallback: "");
    public string MessageMailOwnerError => GetThis(fallback: "");
    public string ButtonSend => GetThis(fallback: "");

    #endregion

    #region Footer Recaptcha
    
    public string MessageContainsRecaptcha => GetThis(fallback: "");
    public string MessageRecaptchaWarning => GetThis(fallback: "");

    #endregion

   

  }
}
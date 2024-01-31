namespace ThisApp.Data
{
  public partial class AppSettings : Custom.Data.Item16
  {
    #region Mailchimp
    public bool MailChimpShowWarning => Bool();
    public string MailchimpServer => String(fallback: "");
    
    public string MailchimpListId => String(fallback: "");
    public string MailchimpAPIKey => String(fallback: "");


    #endregion

    #region SendMail
    public string DefaultMailFrom => String(fallback: "");
    public string DefaultOwnerMail => String(fallback: "");

    #endregion


  }
}
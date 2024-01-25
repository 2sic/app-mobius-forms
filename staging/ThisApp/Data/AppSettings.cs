using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class AppSettings : Custom.Data.Item16Experimental
  {
    public AppSettings(ITypedItem item) : base(item) { }

    #region Mailchimp
    public bool MailChimpShowWarning => GetThis(fallback: false);
    public string MailchimpServer => GetThis(fallback: "")
    ; public string MailchimpListId => GetThis(fallback: "")
    ; public string MailchimpAPIKey => GetThis(fallback: "");


    #endregion

    #region SendMail
    public string DefaultMailFrom => GetThis(fallback: "");
    public string DefaultOwnerMail => GetThis(fallback: "");

    #endregion


  }
}
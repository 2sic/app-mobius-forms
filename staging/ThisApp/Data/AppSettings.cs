using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class AppSettings : Custom.Data.Item16Experimental
  {
    public AppSettings(ITypedItem item) : base(item) { }

    #region Mailchimp
    public bool MailChimpShowWarning => GetThis(fallback: false);

    #endregion

    #region Footer Submit with Messages
    public string DefaultMailFrom => GetThis(fallback: "");
    public string DefaultOwnerMail => GetThis(fallback: "");

    #endregion


  }
}
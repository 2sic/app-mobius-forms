using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public partial class DynForm : Custom.Data.Item16Experimental
  {
    public DynForm(ITypedItem item) : base(item) { }

    #region Dynamic Form

    public string DesignField => GetThis(fallback: "");
    public bool ReuseConfig => GetThis(fallback: false);
    public new string Title => GetThis(fallback: "");
    public bool SkipSubmit => GetThis(fallback: false);
    #endregion

    #region Mailchimp

    public bool Mailchimp => GetThis(fallback: false);

    #endregion

    #region Footer Submit with Messages

    public bool CustomerSend => GetThis(fallback: false);
    public bool OwnerSend => GetThis(fallback: false);
    public string MailFrom => GetThis(fallback: "");
    public string OwnerMail => GetThis(fallback: "");

    #endregion


  }
}
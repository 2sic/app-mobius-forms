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
    public bool Recaptcha => GetThis(fallback: false);

    #endregion

    #region Mailchimp

    public bool Mailchimp => GetThis(fallback: false);

    #endregion

    #region Send Mail
    public string OwnerMailTemplate => GetThis(fallback: "");
    public string OwnerMailCC => GetThis(fallback: "");
    public string CustomerMailTemplate => GetThis(fallback: "");
    public string CustomerMailCC => GetThis(fallback: "");

    public bool CustomerSend => GetThis(fallback: false);
    public bool OwnerSend => GetThis(fallback: false);
    public string MailFrom => GetThis(fallback: "");
    public string OwnerMail => GetThis(fallback: "");

    #endregion

    #region EmailToCustomized

    public string OwnerMailSubject => GetThis(fallback: "");

    #endregion

    #region EmailToCustomer
    public string CustomerMailSubject => GetThis(fallback: "");

    #endregion

  }
}
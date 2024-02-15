using ToSic.Sxc.Data;

namespace AppCode.Data
{
  public class SendMailConfigStack
  {
    public SendMailConfigStack(ITypedStack sendMailConfig)
    {
      SendMailConfig = sendMailConfig;
    }
    protected ITypedStack SendMailConfig { get; }

    public bool OwnerSend => SendMailConfig.Bool(nameof(OwnerSend));
    public bool CustomerSend => SendMailConfig.Bool(nameof(OwnerSend));
    public string OwnerMail => SendMailConfig.String(nameof(OwnerMail), fallback: "");
    public string OwnerMailCC => SendMailConfig.String(nameof(OwnerMailCC), fallback: "");
    public string OwnerMailTemplate => SendMailConfig.String(nameof(OwnerMailTemplate), fallback: "");
    public string CustomerMailCC => SendMailConfig.String(nameof(CustomerMailCC), fallback: "");
    public string CustomerMailTemplate => SendMailConfig.String(nameof(CustomerMailTemplate), fallback: "");
    public string MailFrom => SendMailConfig.String(nameof(MailFrom), fallback: "");

  }

}
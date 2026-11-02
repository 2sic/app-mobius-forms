namespace AppCode.Data
{
  public class SendMailConfigStack
  {
    public SendMailConfigStack(FormSendMailConfig configStack)
    {
      ConfigStack = configStack;
    }
    protected FormSendMailConfig ConfigStack { get; }

    public bool OwnerSend => ConfigStack.Bool(nameof(OwnerSend));
    public bool CustomerSend => ConfigStack.Bool(nameof(CustomerSend));
    public string OwnerMail => ConfigStack.String(nameof(OwnerMail), fallback: "");
    public string OwnerMailCC => ConfigStack.String(nameof(OwnerMailCC), fallback: "");
    public string OwnerMailTemplate => ConfigStack.String(nameof(OwnerMailTemplate), fallback: "");
    public string CustomerMailCC => ConfigStack.String(nameof(CustomerMailCC), fallback: "");
    public string CustomerMailTemplate => ConfigStack.String(nameof(CustomerMailTemplate), fallback: "");
    public string MailFrom => ConfigStack.String(nameof(MailFrom), fallback: "");

  }

}
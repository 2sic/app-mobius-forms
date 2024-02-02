using ToSic.Razor.Html5;

namespace ThisApp.Data
{
  public partial class AppResources { 
    public FormResources DefaultFormResources => _formResources ??= Child<FormResources>(nameof(DefaultFormResources));

    private FormResources _formResources;
    public SendMailConfig DefaultSendMailConfig => _sendMail ??= Child<SendMailConfig>(nameof(DefaultSendMailConfig));
    private SendMailConfig _sendMail;
  }

}
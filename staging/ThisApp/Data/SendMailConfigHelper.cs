using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public class SendMailConfigHelper : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Get formResources back default or from the form
    /// </summary>
    public SendMailConfig GetSendMail(AppResources appRes, SendMailConfig sendMailConfig)
    {
        var fallbackSendMail = appRes.DefaultSendMailConfig;
        var sendMailConfiguration = sendMailConfig ?? fallbackSendMail;
        // sendMailConfi = AsStack(sendMailConfi, fallbackSendMail);
        return sendMailConfiguration;
    }

  }
}
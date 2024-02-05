namespace ThisApp.Data
{
  public class DataStackHelper : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Get formResources back default or from the form
    /// </summary>
    public FormResourcesStack GetFormResources(FormConfig formConfig)
    {
      var fallbackResources = AppResources.DefaultFormResources;
      var formResources = formConfig.FormResources ?? fallbackResources;
      var formResourcesStack = AsStack(formResources?.Entity, fallbackResources?.Entity);
      return new FormResourcesStack(formResourcesStack);
    }

    /// <summary>
    /// Get SendmailConfig back default or from the form
    /// </summary>
    public SendMailConfigStack GetSendMail(SendMailConfig sendMailConfig)
    {
      // TODO: @2dm Logik not Work right now, if no sendMailConfig is set, it will not use the default
      var fallbackSendMail = AppResources.DefaultSendMailConfig;
      var sendMailConfiguration = sendMailConfig ?? fallbackSendMail;
      var sendMailConfigStack = AsStack(sendMailConfiguration?.Entity, fallbackSendMail?.Entity);
      return new SendMailConfigStack(sendMailConfigStack);
    }

    private AppResources AppResources => _appResources ??= As<AppResources>(App.Resources);
    private AppResources _appResources;
  }

}
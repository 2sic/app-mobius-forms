using ThisApp.Data;

namespace ThisApp.Razor
{
  public abstract class MobiusRazorBase : Custom.Hybrid.RazorTyped
  {
    protected AppResources AppRes => _appResources ??= As<AppResources>(App.Resources);
    private AppResources _appResources;

    protected AppSettings AppSet => _appSettings ??= As<AppSettings>(App.Settings);
    private AppSettings _appSettings;

    protected DynForm FormConfig => _dynForm ??= As<DynForm>(MyItem);
    private DynForm _dynForm;

    protected FormResourcesStack FormRes => _formRes ??= DataStack.GetFormResources(FormConfig);
    private FormResourcesStack _formRes;

    protected SendMailConfigStack MailConfig => _sendMailConfig ??= DataStack.GetSendMail(FormConfig.SendMailConfig);
    private SendMailConfigStack _sendMailConfig;

    private DataStackHelper DataStack => _dataStackHelper ??= GetService<DataStackHelper>();
    private DataStackHelper _dataStackHelper;
  }
}
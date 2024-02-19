using AppCode.Data;

namespace AppCode.Razor
{
  public abstract class MobiusRazorBase : Custom.Hybrid.RazorTyped
  {
    /// <summary>
    /// App Resources (typed)
    /// </summary>
    protected AppResources AppRes => _appResources ??= As<AppResources>(App.Resources);
    private AppResources _appResources;

    /// <summary>
    /// App Settings (typed)
    /// </summary>
    protected AppSettings AppSet => _appSettings ??= As<AppSettings>(App.Settings);
    private AppSettings _appSettings;

    /// <summary>
    /// Current form configuration
    /// </summary>
    protected FormConfig FormConfig => _formConfig ??= As<FormConfig>(MyItem);
    private FormConfig _formConfig;

    /// <summary>
    /// Form Resources (typed)
    /// </summary>
    protected FormResourcesStack FormRes => _formRes ??= DataStack.GetFormResources(FormConfig);
    private FormResourcesStack _formRes;

    /// <summary>
    /// Mail Configuration (typed)
    /// </summary>
    protected SendMailConfigStack MailConfig => _sendMailConfig ??= DataStack.GetSendMail(FormConfig.FormSendMailConfig);
    private SendMailConfigStack _sendMailConfig;

    private DataStackHelper DataStack => _dataStackHelper ??= GetService<DataStackHelper>();
    private DataStackHelper _dataStackHelper;

    /// <summary>
    /// Activate the edit experience for the current user.
    /// By default it's active for the system admin, and normal admins only see if if the app setting is enabled.
    /// </summary>
    protected bool EnableEditExperience => _enableEditExperience ??= AppSet.AdminHasPermission || MyContext.User.IsSystemAdmin;
    private bool? _enableEditExperience;

    public Csv.CsvHelper CsvHelper => _csvHelper ??= GetService<Csv.CsvHelper>();
    private AppCode.Csv.CsvHelper _csvHelper;
  }
}
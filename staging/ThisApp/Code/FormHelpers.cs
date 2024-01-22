using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class FormHelpers : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public FormHelpers(IHasCodeContext parent) : base(parent) { }

    /// <summary>
    /// The custom marker so the JS can find the form
    /// </summary>
      // TODO: probably use UniqueId
    public string FormMobiusId() => $"mobius-{MyContext.Module.Id}";

    // The URL to the API endpoint - which uses the current edition (staging/live) and the workflow ID
    public string WebApiUrl()
    {
      return "app/auto/" + MyView.Edition + "/api/DynForm/ProcessForm";
    }

    public string WrapperClasses(ITypedItem formConfig)
    {
      return "app-mobius5-wrapper" + (formConfig.Get<bool>("Mailchimp") ? " app-mobius5-mailchimp" : "");
    }
  }
}
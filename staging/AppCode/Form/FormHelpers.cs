using ToSic.Sxc.Data;

namespace AppCode.Form
{
  public class FormHelpers : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// The custom marker so the JS can find the form
    /// </summary>
    public string FormMobiusId() => $"mobius-{UniqueKey}";

    // The URL to the API endpoint - which uses the current edition (staging/live) and the workflow ID
    public string WebApiUrl()
    {
      return "app/auto/" + MyView.Edition + "/api/Form/ProcessForm";
    }

    public string WrapperClasses(ITypedItem formConfig)
    {
      return "app-mobius6-wrapper" + (formConfig.Get<bool>("Mailchimp") ? " app-mobius6-mailchimp" : "");
    }

  }
}
using ToSic.Sxc.Data;

namespace ThisApp.Form
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
      return "app/auto/" + MyView.Edition + "/api/DynForm/ProcessForm";
    }

    public string WrapperClasses(ITypedItem dynForm)
    {
      return "app-mobius5-wrapper" + (dynForm.Get<bool>("Mailchimp") ? " app-mobius5-mailchimp" : "");
    }
  }
}
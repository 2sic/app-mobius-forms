
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Helpers: Custom.Hybrid.CodeTyped
{
  // The custom marker so the JS can find the form
  public string FormMobiusId() {
    return "mobius-" + UniqueKey;
  }

  // The URL to the API endpoint - which uses the current edition (staging/live) and the workflow ID
  public string WebApiUrl(string workflowId) {
    return "app/auto/" + MyView.Edition + "/api/Form/ProcessForm?workflowId=" + workflowId;
  }

  public string WrapperClasses(ITypedItem formConfig) { 
    return "app-mobius5-wrapper" + (formConfig.Get<bool>("Mailchimp") ? " app-mobius5-mailchimp" : "");
  }
}
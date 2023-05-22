
using ToSic.Razor.Blade;

public class Helpers: Custom.Hybrid.Code14
{
  // The custom marker so the JS can find the form
  public string FormMobiusId() {
    return "mobius-" + CmsContext.Module.Id;
  }

  // The URL to the API endpoint - which uses the current edition (staging/live) and the workflow ID
  public string WebApiUrl(string workflowId) {
    return "app/auto/" + CmsContext.View.Edition + "/api/Form/ProcessForm?workflowId=" + workflowId;
  }

  public string WrapperClasses(dynamic formConfig) { 
    return "app-mobius5-wrapper" + (formConfig.Get<bool>("Mailchimp") ? " app-mobius5-mailchimp" : "");
  }
}
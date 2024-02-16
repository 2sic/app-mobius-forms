namespace AppCode.Form
{
  public class FormHelpers : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// The custom marker so the JS can find the form
    /// </summary>
    public string FormMobiusId() => $"mobius-{UniqueKey}";

    // The URL to the API endpoint - which uses the current edition (staging/live) and the workflow ID
    public string WebApiUrl() => $"app/auto/{MyView.Edition}/api/Form/ProcessForm";

  }
}
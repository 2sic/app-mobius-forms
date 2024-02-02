using ToSic.Sxc.Data;

namespace ThisApp.Data
{
  public class FormResourceHelper : Custom.Hybrid.CodeTyped
  {
    /// <summary>
    /// Get formResources back default or from the form
    /// </summary>
    public FormResources GetFormResources(AppResources appRes, DynForm dynForm)
    {

        var fallbackResources = appRes.DefaultFormResources;
        var formResources = dynForm.FormResources ?? fallbackResources;
        // TODO:: @2dm not same Type?
        // formResources = AsStack(dynForm.FormResources, fallbackResources); 
        return formResources;
    }

  }
}
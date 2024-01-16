using ThisApp.Data;

namespace ThisApp.Code
{
  public class FormBuildParameters
  {
    public FormBuildParameters(AppResources resources, DynForm form, CssClasses cssClasses) {
      Resources = resources;
      Form = form;
      CssClasses = cssClasses;
    }

    public AppResources Resources { get; }
    public DynForm Form { get; }
    public CssClasses CssClasses { get; }

    public bool IsBs3 => CssClasses.IsBs3;
  }
}
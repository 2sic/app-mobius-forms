using ThisApp.Data;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class FormConfiguration
  {
    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public FormConfiguration(AppResources resources, DynForm form, CssClasses cssClasses) {
      Resources = resources;
      Form = form;
      CssClasses = cssClasses;
    }

    public AppResources Resources { get; }
    public DynForm Form { get; }
    public CssClasses CssClasses { get; }
  }
}
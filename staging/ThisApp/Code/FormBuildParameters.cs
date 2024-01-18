using System;
using ThisApp.Data;
using ToSic.Sxc.Context;

namespace ThisApp.Code
{
  public class FormBuildParameters
  {
    public FormBuildParameters(AppResources resources, DynForm form, CssClasses cssClasses, ICmsUser user, bool useFloatingLabels )
    {
      Resources = resources;
      Form = form;
      CssClasses = cssClasses;
      User = user;
      UseFloatingLabels = useFloatingLabels;
    }

    public AppResources Resources { get; }
    public DynForm Form { get; }
    public CssClasses CssClasses { get; }

    public bool IsBs3 => CssClasses.IsBs3;

    public ICmsUser User { get; }
    public Boolean UseFloatingLabels { get; }

  }
}
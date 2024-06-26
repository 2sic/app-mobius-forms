using AppCode.Data;
using ToSic.Sxc.Context;

namespace AppCode.Form
{
  public class FormBuildParameters
  {
    public FormBuildParameters(AppResources resources, FormResourcesStack formResources, SendMailConfigStack sendMailConfig, CssClasses cssClasses, ICmsUser user, bool useFloatingLabels)
    {
      Resources = resources;
      FormResources = formResources;
      SendMailConfig = sendMailConfig;
      CssClasses = cssClasses;
      User = user;
      UseFloatingLabels = useFloatingLabels;
    }

    public AppResources Resources { get; }
    public FormResourcesStack FormResources { get; }

    public SendMailConfigStack SendMailConfig { get; }
    public CssClasses CssClasses { get; }

    public bool IsBs3 => CssClasses.IsBs3;

    public ICmsUser User { get; }
    public bool UseFloatingLabels { get; }

    /// <summary>
    /// Hacky handover to get Kit on all Field Builders
    /// </summary>
    public FormBuilder Builder {get;set;}

  }
}
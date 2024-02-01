using System;
using ThisApp.Data;
using ToSic.Sxc.Context;

namespace ThisApp.Form
{
  public class FormBuildParameters
  {
    public FormBuildParameters(AppResources resources, FormResources formResources, SendMailConfig sendMailConfig, CssClasses cssClasses, ICmsUser user, bool useFloatingLabels )
    {
      Resources = resources;
      FormResources = formResources;
      SendMailConfig = sendMailConfig;
      CssClasses = cssClasses;
      User = user;
      UseFloatingLabels = useFloatingLabels;
    }

    public AppResources Resources { get; }
    public FormResources FormResources { get; }

    public SendMailConfig SendMailConfig { get; }
    public CssClasses CssClasses { get; }

    public bool IsBs3 => CssClasses.IsBs3;

    public ICmsUser User { get; }
    public Boolean UseFloatingLabels { get; }

  }
}
using System.Collections.Generic;
using AppCode.Data;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace AppCode.Form
{
  public class FormBuildParameters
  {
    public FormBuildParameters(AppResources resources, FormResourcesStack formResources,
      SendMailConfigStack sendMailConfig, CssClasses cssClasses, ICmsUser user,
      ServiceKit16 kit,
      bool useFloatingLabels)
    {
      Resources = resources;
      FormResources = formResources;
      SendMailConfig = sendMailConfig;
      CssClasses = cssClasses;
      User = user;
      UseFloatingLabels = useFloatingLabels;
      _kit = kit;
    }

    public AppResources Resources { get; }
    public FormResourcesStack FormResources { get; }
    
    /// <summary>
    /// Service kit for using the TemplateService
    /// </summary>
    private ServiceKit16 _kit;

    public SendMailConfigStack SendMailConfig { get; }
    public CssClasses CssClasses { get; }

    public ICmsUser User { get; }
    public bool UseFloatingLabels { get; }

    /// <summary>
    /// Hacky handover to get Kit on all Field Builders
    /// </summary>
    public FormBuilder Builder {get;set;}


    /// <summary>
    /// Special helper to quickly parse tokens, but only of very restricted sources.
    /// 
    /// See also https://docs.2sxc.org/abyss/parts/look-up/sources.html
    /// </summary>
    /// <param name="template">string optionally containing placeholders such as [QueryString:Id]</param>
    /// <returns></returns>
    public string Parse(string template) {
      var templateSvc = _kit.Template;
      _lookUps ??= new List<ILookUp> {
        templateSvc.GetSource("QueryString"), // will provide url parameters
        templateSvc.GetSource("DateTime"),    // date time info, can be formatted in the output
        templateSvc.GetSource("User"),        // user info, like user name
        templateSvc.GetSource("Site"),        // site info, like site ID
        templateSvc.GetSource("Page"),        // page info, like page ID
        templateSvc.GetSource("Module"),      // module info, like module ID
        templateSvc.GetSource("Ticks"),       // timestamp info, like current ticks
      };
      return templateSvc.Parse(template, sources: _lookUps);
    }

    private List<ILookUp> _lookUps;
  }
}
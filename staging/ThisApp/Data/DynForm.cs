using System.Collections.Generic;
using System.Linq;

namespace ThisApp.Data
{
  public partial class DynForm
  {
    public bool UseFloatingLabels => _useFloatingLabels ??= UseConfigOf.DesignField == "floatingLabel";
    private bool? _useFloatingLabels;

    public List<DynFormField> Fields => _fields ??= UseConfigOf.Children<DynFormField>("Fields").ToList();
    private List<DynFormField> _fields;

    public FormResources FormResources => _formResources ??= UseConfigOf.Child<FormResources>("FormResources");
    private FormResources _formResources;

    public SendMailConfig SendMailConfig => _sendMailConfig ??= UseConfigOf.Child<SendMailConfig>("SendMailConfig");
    private SendMailConfig _sendMailConfig;


    /// <summary>
    /// This is the source of the configs - can be the own object, but can also be the one in "InheritedConfig".
    /// Fallback is "this", in case it doesn't have a child.
    /// </summary>
    private DynForm UseConfigOf => _useConfigOf ??= ReuseConfig ? (InheritedConfig ?? this) : this;
    private DynForm _useConfigOf;

    private DynForm InheritedConfig => _inheritedConfig ??= ReuseConfig ? Child<DynForm>("InheritedConfig") : null;
    private DynForm _inheritedConfig;
  }
}
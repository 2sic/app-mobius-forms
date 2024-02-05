using System.Collections.Generic;
using System.Linq;

namespace ThisApp.Data
{
  public partial class FormConfig
  {
    public bool UseFloatingLabels => _useFloatingLabels ??= UseConfigOf.DesignField == "floatingLabel";
    private bool? _useFloatingLabels;

    public List<FormFieldConfig> Fields => _fields ??= UseConfigOf.Children<FormFieldConfig>("Fields").ToList();
    private List<FormFieldConfig> _fields;

    public FormResources FormResources => _formResources ??= UseConfigOf.Child<FormResources>("FormResources");
    private FormResources _formResources;

    public SendMailConfig SendMailConfig => _sendMailConfig ??= UseConfigOf.Child<SendMailConfig>("SendMailConfig");
    private SendMailConfig _sendMailConfig;


    /// <summary>
    /// This is the source of the configs - can be the own object, but can also be the one in "InheritedConfig".
    /// Fallback is "this", in case it doesn't have a child.
    /// </summary>
    private FormConfig UseConfigOf => _useConfigOf ??= ReuseConfig ? (InheritedConfig ?? this) : this;
    private FormConfig _useConfigOf;

    private FormConfig InheritedConfig => _inheritedConfig ??= ReuseConfig ? Child<FormConfig>("InheritedConfig") : null;
    private FormConfig _inheritedConfig;
  }
}
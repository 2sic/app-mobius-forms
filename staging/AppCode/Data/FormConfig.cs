using System.Collections.Generic;

namespace AppCode.Data
{
  public partial class FormConfig
  {
    public bool UseFloatingLabels => _useFloatingLabels ??= UseConfigOf.DesignField == "floatingLabel";
    private bool? _useFloatingLabels;

    public IEnumerable<FormFieldConfig> Fields => _fields ??= UseConfigOf.Children<FormFieldConfig>("Fields");
    private IEnumerable<FormFieldConfig> _fields;

    // public FormResources FormResources => _formResources ??= UseConfigOf.Child<FormResources>("FormResources");
    // private FormResources _formResources;

    public FormSendMailConfig SendMailConfig => _sendMailConfig ??= UseConfigOf.Child<FormSendMailConfig>("FormSendMailConfig");
    private FormSendMailConfig _sendMailConfig;

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
namespace AppCode.Data
{
  public partial class FormConfig
  {
    public bool UseFloatingLabels => _useFloatingLabels ??= UseConfigOf.DesignField == "floatingLabel";
    private bool? _useFloatingLabels;

    /// <summary>
    /// This is the source of the configs - can be the own object, but can also be the one in "InheritedConfig".
    /// Fallback is "this", in case it doesn't have a child.
    /// </summary>
    private FormConfig UseConfigOf => _useConfigOf ??= ReuseConfig ? (InheritedConfig ?? this) : this;
    private FormConfig _useConfigOf;
  }
}
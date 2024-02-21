namespace AppCode.Data
{
  public partial class FormFieldConfig
  {
    /// <summary>
    /// Use a Razor Component instead of the automatic field builder.
    /// </summary>
    public bool UseRazorComponent => FieldType == "razor";
  }
}
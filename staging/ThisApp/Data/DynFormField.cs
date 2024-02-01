namespace ThisApp.Data
{
  public partial class DynFormField
  {
    /// <summary>
    /// Use a Razor Component instead of the automatic field builder.
    /// </summary>
    public bool UseRazorComponent => FieldType == "advanced";
  }
}
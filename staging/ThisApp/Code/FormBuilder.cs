using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ThisApp.Data;

namespace ThisApp.Code
{
  public class FormBuilder : Custom.Hybrid.CodeTyped
  {
    public FormBuilder(IHasCodeContext parent, FormBuildParameters formParams) : base(parent)
    {
      FormParams = formParams;
    }

    public FormBuildParameters FormParams { get; }

    /// <summary>
    /// Get the HTML for a single field
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public IHtmlTag Field(DynFormField field) => FindBuilder(field)?.GetTag()
        ?? throw new System.Exception($"The type '{field.FieldType}' is not supported yet in the new FormBuilder");

    /// <summary>
    /// Find a matching field builder for the given field
    /// </summary>
    private BuildFieldBase FindBuilder(DynFormField field) => field.FieldType switch
    {
      "string" => new BuildFieldText(FormParams, field),
      "number" => new BuildFieldNumber(FormParams, field),
      "email" => new BuildFieldEMail(FormParams, field),
      _ => null,
    };
  }
}
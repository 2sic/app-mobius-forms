using ThisApp.Data;
using ToSic.Razor.Blade;

namespace ThisApp.Code
{
  public class BuildFieldException : BuildFieldBase
  {
    public BuildFieldException(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Get e exception message if a FieldType available
    /// </summary>
    public override IHtmlTag GetTag() => FileException();

    private IHtmlTag FileException()
    {
      var exception = Tag.Div().Class("alert alert-danger").Attr("role", "alert")
          .Add(Tag.Strong("Exception"))
          .Add(Tag.Ul("FieldTitel: " + Field.Title))
          .Add(Tag.Ul("FieldType: " + Field.FieldType));

      if (!string.IsNullOrEmpty(Field.PickerType))
        exception.Add(Tag.Ul("PickerType: " + Field.PickerType));

      return exception;
    }
  }
}
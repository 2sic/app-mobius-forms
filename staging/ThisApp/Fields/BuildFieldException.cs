using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;

namespace ThisApp.Fields
{
  public class BuildFieldException : BuildFieldBase
  {
    public BuildFieldException(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Get e exception message if a FieldType available
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var specs = Tag.Ul().Wrap(
        Tag.Li("Field Title: " + Field.Title),
        Tag.Li("Field ID: " + Field.FieldId),
        Tag.Li("Field Type: " + Field.FieldType)
      );
      
      if (Text.Has(Field.PickerType))
        specs = specs.Add(Tag.Li("PickerType: " + Field.PickerType));
      
      var exception = Tag.Div()
        .Class("alert alert-danger")
        .Attr("role", "alert")
        .Wrap(
          Tag.Strong("Error: Unknown Field Type"),
          specs
        );

      return exception;
    }
  }
}
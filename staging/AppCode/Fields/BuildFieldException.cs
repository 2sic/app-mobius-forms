using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldException : BuildFieldBase
  {
    public BuildFieldException(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Get e exception message if a FieldType available
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var tags = Builder.Kit.HtmlTags;
      var specs = tags.Ul().Wrap(
        tags.Li("Field Title: " + Field.Title),
        tags.Li("Field ID: " + Field.FieldId),
        tags.Li("Field Type: " + Field.FieldType)
      );

      if (Field.IsNotEmpty("PickerType"))
        specs = specs.Add(tags.Li("PickerType: " + Field.PickerType));

      var exception = tags.Div()
        .Class("alert alert-danger")
        .Attr("role", "alert")
        .Wrap(
          tags.Strong("Error: Unknown Field Type"),
          specs
        );

      return exception;
    }
  }
}
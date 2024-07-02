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
      var tag = Builder.Kit.HtmlTags;
      var specs = tag.Ul().Wrap(
        tag.Li("Field Title: " + Field.Title),
        tag.Li("Field ID: " + Field.FieldId),
        tag.Li("Field Type: " + Field.FieldType)
      );

      if (Field.IsNotEmpty("PickerType"))
        specs = specs.Add(tag.Li("PickerType: " + Field.PickerType));

      var exception = tag.Div()
        .Class("alert alert-danger")
        .Attr("role", "alert")
        .Wrap(
          tag.Strong("Error: Unknown Field Type"),
          specs
        );

      return exception;
    }
  }
}
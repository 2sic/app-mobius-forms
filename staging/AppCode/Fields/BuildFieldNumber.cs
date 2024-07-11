using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldNumber : BuildFieldBase
  {
    public BuildFieldNumber(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Generate a Number Input with Min, Max and Value
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(NumberField());

    private Input NumberField()
    {
      var tags = Builder.Kit.HtmlTags;
      var item = tags.Input().Type("number");

      var minValue = Field.NumberMin;
      var maxValue = Field.NumberMax;
      var value = Form.Parse(Field.DefaultValue);

      if (minValue != 0) item = item.Min(minValue.ToString());
      if (maxValue != 0) item = item.Max(maxValue.ToString());
      if (value != null) item = item.Attr("value", value);
      return item;
    }
  }
}
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
      var tag = Builder.Kit.HtmlTags;
      var item = tag.Input().Type("number");

      var minValue = Field.NumberMin;
      var maxValue = Field.NumberMax;
      var value = Field.DefaultValue;

      if (minValue != 0) { item.Min(minValue.ToString()); }
      if (maxValue != 0) { item.Max(maxValue.ToString()); }
      if (value != null) { item.Attr("value", value); }
      return item;
    }
  }
}
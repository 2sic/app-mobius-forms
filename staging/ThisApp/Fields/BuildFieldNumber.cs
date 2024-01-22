using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  public class BuildFieldNumber : BuildFieldBase
  {
    public BuildFieldNumber(FormBuildParameters form, DynFormField field) : base(form, field) { }
    /// <summary>
    /// Generate a Number Input with Min, Max and Value
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(NumberField());

    private Input NumberField()
    {
      var item = Tag.Input().Type("number");

      var minValue = Field.MinLength;
      var maxValue = Field.MaxLength;
      var value = Field.InitialValue;

      if (minValue != 0) { item.Min(minValue.ToString()); }
      if (maxValue != 0) { item.Max(maxValue.ToString()); }
      if (value != null) { item.Attr("value", value); }

      return item;
    }
  }
}
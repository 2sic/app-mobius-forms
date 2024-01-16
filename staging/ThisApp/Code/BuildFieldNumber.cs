using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldNumber : BuildFieldBase
  {
    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public BuildFieldNumber(FormConfiguration form, DynFormField field) : base(form, field) { }

    public override Input GetInput()
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
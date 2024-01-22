using System;
using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldTextMultiline : BuildFieldBase
  {
    public BuildFieldTextMultiline(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Text must override it, because the MultiLine is not an input, but a textarea
    /// </summary>
    public override IHtmlTag GetTag()
    {
      var item = MultiLine();
      item = SetBasicsMultiLine(item);
      return WrapInLabel(item);
    }

    private Textarea MultiLine()
    {
      var rows = Field.StringLines;
      var item = Tag.Textarea().Rows(rows.ToString());
      if (Text.Has(Field.InitialValue)) { item.Add(Field.InitialValue); }
      return item;
    }

    protected Textarea SetBasicsMultiLine(Textarea item)
    {
      var result = item
        .Id(Field.FieldId)
        .Placeholder(PlaceholderLabel())
        .Class(CssClasses.InputControl);

      if (Field.Required) result = SetRequired(result);
      if (Field.IsDisabled) result = result.Disabled();
      return result;
    }

  }
}
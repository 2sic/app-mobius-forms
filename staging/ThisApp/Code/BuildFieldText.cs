using System;
using System.Dynamic;
using DotNetNuke.UI.WebControls;
using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldText : BuildFieldBase
  {
    /// <summary>
    /// Constructor which ensures that this class has the same context as the parent, eg. the Kit etc.
    /// </summary>
    public BuildFieldText(FormConfiguration form, DynFormField field) : base(form, field) {
    }

    /// <summary>
    /// Text must override it, because the MultiLine is not an input, but a textarea
    /// </summary>
    /// <returns></returns>
    public override IHtmlTag GetTag()
    {
      var rows = Field.StringLines;
      if (rows <= 1) {
        var item = SingleLine();
        item = SetBasics(item);
        return WrapInLabel(item);
      }
      var itemMl = MultiLine(rows);
      itemMl = SetBasicsMultiLine(itemMl);
      return WrapInLabel(itemMl);
    }

    public override Input GetInput() => throw new NotImplementedException();

    private Input SingleLine()
    {
      var item = Tag.Input().Type("text");
      if (Text.Has(Field.InitialValue)) { item.Attr("value", Field.InitialValue); }
      return item;
    }

    private Textarea MultiLine(int rows)
    {
      var item = Tag.Textarea().Rows(rows.ToString());
      if (Text.Has(Field.InitialValue)) { item.Attr("value", Field.InitialValue); }
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
using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldTextMultiline : BuildFieldBase
  {
    public BuildFieldTextMultiline(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

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
      var tags = Builder.Kit.HtmlTags;  
      var rows = Field.StringLines;
      var item = tags.Textarea().Rows(rows.ToString());
      if (Field.IsNotEmpty("DefaultValue"))
        item = item.Add(Form.Parse(Field.DefaultValue));
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
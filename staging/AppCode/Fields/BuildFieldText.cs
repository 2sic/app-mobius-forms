using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldText : BuildFieldBase
  {
    public BuildFieldText(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Text must override GetTag(),
    /// because the MultiLine variant is a TextArea (not input)
    /// so it can't be handled in the GetInput() method
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(TextField());

    private Input TextField()
    {
      var tags = Builder.Kit.HtmlTags;
      var item = tags.Input().Type("text");
      if (Field.IsNotEmpty(nameof(Field.DefaultValue)))
        item = item.Value(Form.Parse(Field.DefaultValue));

      return item;
    }
  }
}
using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using System;

namespace AppCode.Fields
{
  public class BuildFieldDate : BuildFieldBase
  {
    public BuildFieldDate(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Date must override GetTag(),
    /// because the MultiLine variant is a TextArea (not input)
    /// so it can't be handled in the GetInput() method
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(DateField());

    private Input DateField()
    {
      var today = DateTime.Today;
      var tags = Builder.Kit.HtmlTags;
      var item = tags.Input().Type("date");

      if (Field.IsNotEmpty(nameof(Field.DefaultValue)))
        item = item.Value(Form.Parse(Field.DefaultValue));

      item = item.Attr("min", today.ToString("yyyy-MM-dd"));

      return item;
    }
  }
}
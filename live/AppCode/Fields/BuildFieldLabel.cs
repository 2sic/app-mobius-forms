using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;

namespace AppCode.Fields
{
  public class BuildFieldLabel : BuildFieldBase
  {
    public BuildFieldLabel(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Show only the Field Title as Label (Settings open (Size, Position, etc.))
    /// </summary>
    public override IHtmlTag GetTag() => FieldLabel();

    private IHtmlTag FieldLabel()
    {
      var tag = Builder.Kit.HtmlTags;
      var label = tag.H4(Field.Title);
      return label;
    }
  }
}
using ThisApp.Data;
using ToSic.Razor.Blade;

namespace ThisApp.Code
{
  public class BuildFieldLabel : BuildFieldBase
  {
    public BuildFieldLabel(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => FieldLabel();

    private IHtmlTag FieldLabel()
    {
      var label = Tag.H4(Field.Title).Class(CssClasses.Label);
      return label;
    }
  }
}
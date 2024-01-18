using ThisApp.Data;
using ToSic.Razor.Blade;

namespace ThisApp.Code
{
  public class BuildFieldLabel : BuildFieldBase
  {
    public BuildFieldLabel(FormBuildParameters form, DynFormField field) : base(form, field) { }
    /// <summary>
    /// Show only the Field Title as Label (Settings open (Size, Position, etc.))
    /// </summary>
    public override IHtmlTag GetTag() => FieldLabel();

    private IHtmlTag FieldLabel()
    {
      var label = Tag.H4(Field.Title);
      return label;
    }
  }
}
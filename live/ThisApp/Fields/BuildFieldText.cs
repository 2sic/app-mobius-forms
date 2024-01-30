using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  public class BuildFieldText : BuildFieldBase
  {
    public BuildFieldText(FormBuildParameters form, DynFormField field) : base(form, field) { }

    /// <summary>
    /// Text must override GetTag(),
    /// because the MultiLine variant is a TextArea (not input)
    /// so it can't be handled in the GetInput() method
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(TextField());

    private Input TextField()
    {
      var item = Tag.Input().Type("text");
      if (Text.Has(Field.InitialValue)) { item.Value(Field.InitialValue); }
      return item;
    }
  }
}
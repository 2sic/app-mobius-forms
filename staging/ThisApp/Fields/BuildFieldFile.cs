using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  public class BuildFieldFile : BuildFieldBase
  {
    public BuildFieldFile(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Build a File Upload Input
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(FileField());

    private Input FileField()
    {
      var input = Tag.Input().Type("file").Name(Field.Title).Class("form-control-file");
      input = SetBasics(input, false);
      return input;
    }

  }
}
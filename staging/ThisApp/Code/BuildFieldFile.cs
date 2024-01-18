using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldFile : BuildFieldBase
  {
    public BuildFieldFile(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(FileField());

    private Input FileField()
    {
      var input = Tag.Input().Type("file").Name(Field.Title).Class("form-control-file");

      // TODO:: Open acceptType ?
      // if (Text.Has(acceptType))
      //   input = input.Attr("accept", acceptType);

      input = SetBasics(input, false);
      return input;

    }

  }
}
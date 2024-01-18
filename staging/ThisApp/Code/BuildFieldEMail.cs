using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldEMail : BuildFieldBase
  {
    public BuildFieldEMail(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(EmailField());

    private Input EmailField()
    {
      var item = Tag.Input().Type("email");
      if (Text.Has(Field.RecipientEmail)) { item.Attr("mail", "recipientEmail"); }
      return item;
    }
  }
}
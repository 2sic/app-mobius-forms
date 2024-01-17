using ThisApp.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Code
{
  public class BuildFieldEMail : BuildFieldBase
  {
    public BuildFieldEMail(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override Input GetInput()
    {
      var item = Tag.Input().Type("email"); // .Id(idString).Placeholder(PhLabel(label, required)).Class(CssClasses.InputControl);
      // SetRequired(item, required, "LabelValidEmail");
      
      if (Text.Has(Field.RecipientEmail)) { item.Attr("mail", "recipientEmail"); }

      return item; // WrapWithLabel(idString, required, item, label);
    }
  }
}
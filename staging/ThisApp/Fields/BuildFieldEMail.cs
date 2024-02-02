using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  public class BuildFieldEMail : BuildFieldBase
  {
    public BuildFieldEMail(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }

    /// <summary>
    /// Build Email field with RecipientEmail Settings
    /// </summary>
    public override IHtmlTag GetTag() => SetBasicsAndWrapInLabel(EmailField());

    private Input EmailField()
    {
      var item = Tag.Input().Type("email");
      if (Text.Has(Field.EmailUseAsRecipient)) { item.Attr("mail", "recipientEmail"); }
      return item;
    }
  }
}
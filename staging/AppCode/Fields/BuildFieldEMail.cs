using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
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
      var tag = Builder.Kit.HtmlTags;
      var item = tag.Input().Type("email");
      if (Field.EmailUseAsRecipient) { item.Attr("mail", "recipientEmail"); }
      return item;
    }
  }
}
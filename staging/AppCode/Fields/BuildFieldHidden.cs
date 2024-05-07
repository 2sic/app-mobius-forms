using AppCode.Data;
using AppCode.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.Fields
{
  public class BuildFieldHidden : BuildFieldBase
  {
    public BuildFieldHidden(FormBuildParameters form, FormFieldConfig field) : base(form, field) { }
    /// <summary>
    /// Generate a Hidden Input field with Value for E-mail and Db, only visible for Admins
    /// </summary>
    public override IHtmlTag GetTag()
    {
      if (Form.User.IsContentAdmin)
        return SetBasicsAndWrapInLabel(FileHiddenAdmin(), setDefaultClass: false);
      else
        return FileHidden();
    }

    private Input FileHiddenAdmin()
    {
      var tag = Builder.Kit.HtmlTags;
      return tag.Input().Type("text").Value(Field.DefaultValue).Disabled().Class(CssClasses.HiddenInputStyle + " " + CssClasses.InputControl);
    }

    private IHtmlTag FileHidden()
    {
      var tag = Builder.Kit.HtmlTags;
      return tag.Div(SetBasics(tag.Input().Type("hidden").Value(Field.DefaultValue), false));
    }
  }
}
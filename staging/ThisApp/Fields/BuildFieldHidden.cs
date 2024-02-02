using Connect.Koi.Html;
using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace ThisApp.Fields
{
  public class BuildFieldHidden : BuildFieldBase
  {
    public BuildFieldHidden(FormBuildParameters form, DynFormField field) : base(form, field) { }
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
      return Tag.Input().Type("text").Value(Field.DefaultValue).Disabled().Class(CssClasses.HiddenInputStyle + " " + CssClasses.InputControl);
    }
    
    private IHtmlTag FileHidden()
    {
      return Tag.Div(SetBasics(Tag.Input().Type("hidden").Value(Field.DefaultValue), false));
    }
  }
}
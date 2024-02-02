using ThisApp.Data;
using ThisApp.Form;
using ToSic.Razor.Blade;

namespace ThisApp.Fields
{
  public class BuildFieldHidden : BuildFieldBase
  {
    public BuildFieldHidden(FormBuildParameters form, DynFormField field) : base(form, field) { }
    /// <summary>
    /// Generate a Hidden Input field with Value for E-mail and Db, only visible for Admins
    /// </summary>
    public override IHtmlTag GetTag() => FileHidden();

    private IHtmlTag FileHidden()
    {
      var field = Form.User.IsContentAdmin
      // If user is admin, show hidden field with warning and Input Filed
          ? Tag.Div().Class("alert alert-warning").Attr("role", "alert")
              .Add(
                  Tag.Strong("Show Hidden Field for Admin"),
                  SetBasics(Tag.Input().Type("text").Value(Field.DefaultValue).Disabled())
              )
          // Else don't show hidden field and send only the Information
          : Tag.Div(SetBasics(Tag.Input().Type("hidden").Value(Field.DefaultValue), false));

      if (!string.IsNullOrEmpty(Field.Title))
        field.Add(Tag.Label(Field.Title).Attr("hidden").For(Field.FieldId));
      return field;
    }
  }
}
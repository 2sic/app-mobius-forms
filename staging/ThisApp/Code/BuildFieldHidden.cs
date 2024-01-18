using DotNetNuke.Modules.Admin.Users;
using ThisApp.Data;
using ToSic.Razor.Blade;

namespace ThisApp.Code
{
  public class BuildFieldHidden : BuildFieldBase
  {
    public BuildFieldHidden(FormBuildParameters form, DynFormField field) : base(form, field) { }

    public override IHtmlTag GetTag() => FileHidden();

    private IHtmlTag FileHidden()
    {
      var field = Form.User.IsContentAdmin
      // If user is admin, show hidden field with warning and Input Filed
          ? Tag.Div().Class("alert alert-warning").Attr("role", "alert")
              .Add(
                  Tag.Strong("Show Hidden Field for Admin"),
                  SetBasics(Tag.Input().Type("text").Value(Field.HiddenValue).Disabled())
              )
          // Else don't show hidden field and send only the Information
          : Tag.Div(SetBasics(Tag.Input().Type("hidden").Value(Field.HiddenValue), false));

      if (!string.IsNullOrEmpty(Field.Title))
        field.Add(Tag.Label(Field.Title).Attr("hidden").For(Field.FieldId));
      return field;
    }
  }
}